extern crate libc;

// calculate blake3 hash from input to output
#[no_mangle]
pub extern fn calculate_blake3(
  in_size: libc::size_t,
  in_pointer: *const u8,
  out_size: libc::size_t, 
  out_pointer: *mut u8)
{
  let input = unsafe { std::slice::from_raw_parts(in_pointer as *const u8, in_size as usize) };
  let output= unsafe { std::slice::from_raw_parts_mut(out_pointer as *mut u8, out_size as usize) };
  impl_calculate_blake3(input, output);
}

// create blake3 hasher
#[no_mangle]
pub extern fn create_blake3() -> *mut blake3::Hasher {
  let hash = blake3::Hasher::new();
  let hash = Box::new(hash);

  Box::into_raw(hash)
}

// delete blake3 hasher
#[no_mangle]
pub extern fn delete_blake3(hasher: *mut blake3::Hasher) {
  unsafe{ Box::from_raw(hasher) };
}

// update blake3 hash
#[no_mangle]
pub extern fn update_blake3(
  hasher: *mut blake3::Hasher,
  in_size: libc::size_t,
  in_pointer: *const u8)
{
  let input = unsafe { std::slice::from_raw_parts(in_pointer as *const u8, in_size as usize) };
  impl_update_blake3(hasher, input);
}

// finalize hash calculating then put result into output.
pub extern fn finalize_blake3(
  hasher: *mut blake3::Hasher,
  out_size: libc::size_t,
  out_pointer: *mut u8)
{
  let output = unsafe { std::slice::from_raw_parts_mut(out_pointer as *mut u8, out_size as usize) };
  impl_finalize_blake3(hasher, output)
}


// implementation calculate_blake3
fn impl_calculate_blake3(input: &[u8], output: &mut [u8]) {
    blake3::Hasher::new().update(input).finalize_xof().fill(output);
}

// implementation update_blake3
fn impl_update_blake3(hasher: *mut blake3::Hasher, input: &[u8]) {
    let hasher = unsafe { &mut *hasher };
    hasher.update(input);
}

// implementation finalize_blake3
fn impl_finalize_blake3(hasher: *mut blake3::Hasher, output: &mut [u8]) {
    let hasher = unsafe { &mut *hasher };
    hasher.finalize_xof().fill(output);
}

#[test]
fn run_test_createdelete(){
  let hasher = create_blake3();
  delete_blake3(hasher);
}
#[test]
fn run_test_calculate_blake3(){
  let key = b"abcde";
  let mut hasher = blake3::Hasher::new();
  hasher.update(key);
  let mut out1 = [0;32];
  hasher.finalize_xof().fill(&mut out1);
  let mut out2 = [0;32];
  impl_calculate_blake3(key, &mut out2);
  assert_eq!(out1, out2);
}
#[test]
fn run_test_fail_calculate_blake3(){
  let mut out1 = [0;32];
  let mut out2 = [0;32];
  impl_calculate_blake3(b"abcde", &mut out1);
  impl_calculate_blake3(b"fghij", &mut out2);
  assert_ne!(out1, out2);
}
#[test]
fn run_test_update_blake3(){
  let hasher = create_blake3();
  let key = b"abcde";
  let mut out1 = [0;32];
  let mut out2 = [0;32];

  impl_update_blake3(hasher, key);
  impl_finalize_blake3(hasher, &mut out1);

  delete_blake3(hasher);

  impl_calculate_blake3(key, &mut out2);
  assert_eq!(out1, out2);
}