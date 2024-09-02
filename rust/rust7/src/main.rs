use std::collections::HashMap;
use std::io::{self, Write};
use std::collections::*;  // minor change to test git

pub mod front_of_house;

fn main() {
    println!("Hello, world!");
    let mut map = HashMap::new();
    map.insert(1, 2);
    front_of_house::hosting::add_to_waitlist();
}
