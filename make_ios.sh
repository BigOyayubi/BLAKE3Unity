#!/bin/sh

# cargo needs to add targets for ios
rustup target add aarch64-apple-ios armv7-apple-ios armv7s-apple-ios x86_64-apple-ios i386-apple-ios

# to generate the iOS universal library.
# cargo install cargo-lipo
# cargo lipo --release

TARGET=aarch64-apple-ios
PLATFORM=iOS
LIBNAME=blake3unity

cargo clean
cargo build --release --target=${TARGET}
mkdir -p Plugins/${PLATFORM}/
cp target/${TARGET}/release/lib${LIBNAME}.a Plugins/${PLATFORM}/lib${LIBNAME}.a
