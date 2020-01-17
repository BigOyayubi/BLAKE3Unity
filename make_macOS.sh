#!/bin/sh

TARGET=x86_64-apple-darwin
cargo clean
cargo build --release --target $TARGET
mkdir -p Plugins/macOS/
cp target/${TARGET}/release/libblake3unity.dylib Plugins/macOS/blake3unity.bundle
