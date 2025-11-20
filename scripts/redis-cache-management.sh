#!/bin/bash

# =============================================
# Redis Cache Management Script
# Tours and Activities API - Cache Operations
# =============================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
REDIS_HOST="${REDIS_HOST:-localhost}"
REDIS_PORT="${REDIS_PORT:-6379}"
REDIS_PASSWORD="${REDIS_PASSWORD:-}"

# Functions
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

# Check if Redis CLI is installed
check_redis_cli() {
    if ! command -v redis-cli &> /dev/null; then
        print_error "redis-cli is not installed"
        exit 1
    fi
    print_success "redis-cli is installed"
}

# Test Redis connection
test_connection() {
    print_info "Testing Redis connection..."
    
    if [ -n "$REDIS_PASSWORD" ]; then
        REDIS_CMD="redis-cli -h $REDIS_HOST -p $REDIS_PORT -a $REDIS_PASSWORD"
    else
        REDIS_CMD="redis-cli -h $REDIS_HOST -p $REDIS_PORT"
    fi
    
    if $REDIS_CMD ping > /dev/null 2>&1; then
        print_success "Connected to Redis at $REDIS_HOST:$REDIS_PORT"
    else
        print_error "Failed to connect to Redis"
        exit 1
    fi
}

# Get cache statistics
get_stats() {
    print_info "Fetching Redis statistics..."
    
    echo ""
    echo "=== Redis Info ==="
    $REDIS_CMD INFO stats | grep -E "total_commands_processed|total_connections_received|keyspace_hits|keyspace_misses"
    
    echo ""
    echo "=== Memory Usage ==="
    $REDIS_CMD INFO memory | grep -E "used_memory_human|used_memory_peak_human|maxmemory_human"
    
    echo ""
    echo "=== Key Statistics ==="
    $REDIS_CMD DBSIZE
}

# Clear all cache
clear_all_cache() {
    print_info "Clearing all cache..."
    read -p "Are you sure you want to clear ALL cache? (yes/no): " confirm
    
    if [ "$confirm" = "yes" ]; then
        $REDIS_CMD FLUSHALL
        print_success "All cache cleared"
    else
        print_info "Operation cancelled"
    fi
}

# Clear cache by pattern
clear_cache_pattern() {
    local pattern=$1
    
    if [ -z "$pattern" ]; then
        print_error "Pattern is required"
        return 1
    fi
    
    print_info "Clearing cache matching pattern: $pattern"
    
    local keys=$($REDIS_CMD KEYS "$pattern")
    local count=$(echo "$keys" | wc -l)
    
    if [ -n "$keys" ]; then
        echo "$keys" | xargs $REDIS_CMD DEL
        print_success "Deleted $count keys matching pattern: $pattern"
    else
        print_info "No keys found matching pattern: $pattern"
    fi
}

# Warm up cache
warm_cache() {
    print_info "Warming up cache..."
    
    # Example: Pre-load popular destinations
    print_info "Pre-loading popular destinations..."
    
    # This would typically call your API endpoints
    # For demonstration purposes:
    print_info "Cache warm-up would trigger API calls here"
    print_success "Cache warm-up completed"
}

# Monitor cache in real-time
monitor_cache() {
    print_info "Monitoring Redis in real-time (Press Ctrl+C to stop)..."
    $REDIS_CMD MONITOR
}

# Get cache keys by pattern
list_keys() {
    local pattern=${1:-"*"}
    
    print_info "Listing keys matching pattern: $pattern"
    $REDIS_CMD KEYS "$pattern"
}

# Get cache hit rate
get_hit_rate() {
    print_info "Calculating cache hit rate..."
    
    local hits=$($REDIS_CMD INFO stats | grep keyspace_hits | cut -d: -f2 | tr -d '\r')
    local misses=$($REDIS_CMD INFO stats | grep keyspace_misses | cut -d: -f2 | tr -d '\r')
    
    if [ -n "$hits" ] && [ -n "$misses" ]; then
        local total=$((hits + misses))
        if [ $total -gt 0 ]; then
            local hit_rate=$(awk "BEGIN {printf \"%.2f\", ($hits / $total) * 100}")
            echo "Cache Hit Rate: $hit_rate%"
            echo "Hits: $hits"
            echo "Misses: $misses"
        else
            print_info "No cache operations recorded yet"
        fi
    fi
}

# Main menu
show_menu() {
    echo ""
    echo "==================================="
    echo "Redis Cache Management"
    echo "==================================="
    echo "1. Test Connection"
    echo "2. Get Statistics"
    echo "3. Get Hit Rate"
    echo "4. List Keys"
    echo "5. Clear Cache by Pattern"
    echo "6. Clear All Cache"
    echo "7. Warm Cache"
    echo "8. Monitor Real-time"
    echo "9. Exit"
    echo "==================================="
}

# Main execution
main() {
    check_redis_cli
    
    if [ $# -eq 0 ]; then
        # Interactive mode
        while true; do
            show_menu
            read -p "Select an option: " choice
            
            case $choice in
                1) test_connection ;;
                2) get_stats ;;
                3) get_hit_rate ;;
                4) 
                    read -p "Enter pattern (default: *): " pattern
                    list_keys "${pattern:-*}"
                    ;;
                5)
                    read -p "Enter pattern: " pattern
                    clear_cache_pattern "$pattern"
                    ;;
                6) clear_all_cache ;;
                7) warm_cache ;;
                8) monitor_cache ;;
                9) 
                    print_info "Goodbye!"
                    exit 0
                    ;;
                *) print_error "Invalid option" ;;
            esac
        done
    else
        # Command-line mode
        case $1 in
            test) test_connection ;;
            stats) get_stats ;;
            hitrate) get_hit_rate ;;
            list) list_keys "${2:-*}" ;;
            clear) clear_cache_pattern "$2" ;;
            clearall) clear_all_cache ;;
            warm) warm_cache ;;
            monitor) monitor_cache ;;
            *) 
                echo "Usage: $0 {test|stats|hitrate|list|clear|clearall|warm|monitor}"
                exit 1
                ;;
        esac
    fi
}

main "$@"

