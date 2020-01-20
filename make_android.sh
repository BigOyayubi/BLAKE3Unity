#!/bin/sh

# cargo needs to add targets for android
rustup target add aarch64-linux-android armv7-linux-androideabi i686-linux-android

# setup android ndk toolchains
MAKE_TOOLCHAIN=${ANDROID_NDK_ROOT}/build/tools/make_standalone_toolchain.py
mkdir NDK
${MAKE_TOOLCHAIN} --api 26 --arch arm64 --install-dir NDK/arm64
${MAKE_TOOLCHAIN} --api 26 --arch arm   --install-dir NDK/arm

# setup cargo config
mkdir .cargo
UNAME="$(uname -s)"
case "${UNAME}" in
  CYGWIN*) CONF=android_win.config;;
  *)       CONF=android_nix.config;;
esac
cp config/${CONF} .cargo/config

# build lib
TARGETS=(aarch64-linux-android armv7-linux-androideabi)
PLATFORM=Android
LIBNAME=blake3unity

cargo clean
mkdir -p Plugins/${PLATFORM}/

# copy lib
for TARGET in ${TARGETS[@]}
do
  cargo build --release --target=${TARGET}
  case "${TARGET}" in
    aarch64-linux-android*)   DST=Plugins/${PLATFORM}/arm64-v8a/lib${LIBNAME}.so;;
    armv7-linux-androideabi*) DST=Plugins/${PLATFORM}/armeabi-v7a/lib${LIBNAME}.so;;
  esac
  mkdir -p "$(dirname ${DST})"
  cp target/${TARGET}/release/lib${LIBNAME}.so ${DST}
done

