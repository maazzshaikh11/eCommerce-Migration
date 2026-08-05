#!/usr/bin/env bash
# NUKE build system entry (bash)
# Delegates to the Makefile for common tasks.
# Usage: ./NUKE.sh <target>  (e.g., ./NUKE.sh build)

set -e

if [ $# -eq 0 ]; then
    echo "Usage: ./NUKE.sh <target>"
    echo "Targets: build, test, lint, docker-up, docker-down, clean, restore"
    exit 1
fi

target="$1"
echo "Running: make $target"
make "$target"