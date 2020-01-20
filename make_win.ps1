#!/bin/sh

$TARGET="x86_64-pc-windows-msvc"

rustup target add $TARGET
cargo clean
cargo build --release --target $TARGET

$DST="Plugins/x86_64/"
if (!(Test-Path $DST)) {
    mkdir -p $DST
}
cp "target/$TARGET/release/blake3unity.dll" $DST

