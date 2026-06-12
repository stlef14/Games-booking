#!/usr/bin/env bash
set -Eeuo pipefail

# -----------------------------------------------------------------------------
# Configuration and Constants
# -----------------------------------------------------------------------------
readonly SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)"
readonly SCRIPT_NAME="$(basename -- "${BASH_SOURCE[0]}")"
readonly SOLUTION_PATH="$SCRIPT_DIR/GamesBooking.sln"

# -----------------------------------------------------------------------------
# Logging Functions
# -----------------------------------------------------------------------------
log_info() {
    printf "[$(date +'%Y-%m-%d %H:%M:%S')] \x1b[34mINFO\x1b[0m: %s\n" "$*" >&2
}

log_success() {
    printf "[$(date +'%Y-%m-%d %H:%M:%S')] \x1b[32mSUCCESS\x1b[0m: %s\n" "$*" >&2
}

log_warn() {
    printf "[$(date +'%Y-%m-%d %H:%M:%S')] \x1b[33mWARN\x1b[0m: %s\n" "$*" >&2
}

log_error() {
    printf "[$(date +'%Y-%m-%d %H:%M:%S')] \x1b[31mERROR\x1b[0m: %s\n" "$*" >&2
}

# -----------------------------------------------------------------------------
# Error Handling
# -----------------------------------------------------------------------------
handle_error() {
    local -r line_num="$1"
    log_error "Command failed at line $line_num. Exiting."
}

trap 'handle_error $LINENO' ERR

# -----------------------------------------------------------------------------
# Help and Usage
# -----------------------------------------------------------------------------
usage() {
    cat <<EOF
Usage: $SCRIPT_NAME [OPTIONS]

A professional build and test runner for the GamesBooking solution.

Options:
    -c, --clean         Clean build artifacts before building
    -t, --test          Run all unit and integration tests
    -r, --release       Build in Release configuration (default is Debug)
    -d, --dry-run       Dry-run: log commands without executing them
    -h, --help          Show this help message
EOF
    exit "${1:-0}"
}

# -----------------------------------------------------------------------------
# Dependency Validation
# -----------------------------------------------------------------------------
check_dependencies() {
    if ! command -v dotnet &>/dev/null; then
        log_error ".NET CLI (dotnet) is not installed or not in PATH."
        exit 1
    fi
}

# -----------------------------------------------------------------------------
# Command Executor (Supports Dry-Run)
# -----------------------------------------------------------------------------
run_cmd() {
    if [[ "${DRY_RUN:-false}" == "true" ]]; then
        log_info "[DRY-RUN] Would run: $*"
        return 0
    fi
    "$@"
}

# -----------------------------------------------------------------------------
# Main Execution Flow
# -----------------------------------------------------------------------------
main() {
    # Default values
    local clean_flag=false
    local run_tests=false
    local config="Debug"
    local DRY_RUN=false

    # Parse arguments
    while [[ $# -gt 0 ]]; do
        case "$1" in
            -c|--clean)
                clean_flag=true
                shift
                ;;
            -t|--test)
                run_tests=true
                shift
                ;;
            -r|--release)
                config="Release"
                shift
                ;;
            -d|--dry-run)
                DRY_RUN=true
                shift
                ;;
            -h|--help)
                usage 0
                ;;
            *)
                log_error "Unknown option: $1"
                usage 1
                ;;
        esac
    done

    # Setup dry-run visibility inside functions
    export DRY_RUN

    # Check dependencies
    check_dependencies

    # Navigate to script directory
    cd "$SCRIPT_DIR"

    # 1. Clean if requested
    if [[ "$clean_flag" == "true" ]]; then
        log_info "Cleaning build artifacts..."
        run_cmd dotnet clean "$SOLUTION_PATH" --configuration "$config"
    fi

    # 2. Restore dependencies
    log_info "Restoring NuGet packages..."
    run_cmd dotnet restore "$SOLUTION_PATH"

    # 3. Build Solution
    log_info "Building solution ($config)..."
    run_cmd dotnet build "$SOLUTION_PATH" --configuration "$config" --no-restore

    # 4. Run Tests if requested
    if [[ "$run_tests" == "true" ]]; then
        log_info "Running tests ($config)..."
        run_cmd dotnet test "$SOLUTION_PATH" --configuration "$config" --no-build
    fi

    log_success "Build workflow completed successfully."
}

main "$@"
