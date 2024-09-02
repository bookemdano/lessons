
// regular enums
enum IpAddrKind {
    V4,
    V6,
}

enum Message {
    Quit,
    Move { x: i32, y: i32 },
    Write(String),
    ChangeColor(i32, i32, i32),
}

// you can add methods to enums
impl Message {
    fn call(&self) {
        // method body would be defined here
    }
}
// enum that takes a string
#[derive(Debug)]
enum IpAddr {
    V4(u8, u8, u8, u8),
    V6(String),
}

fn main() {
    //println!("abcd to {}", remove_duplicates(String::from("abcd"), 2));
    println!("aa to {}", remove_duplicates(String::from("deeedbbcccbdaa"), 3));

    let home = IpAddr::V4(127,0,0,1);
    println!("home {:?}", home);
    let loopback = IpAddr::V6(String::from("::1"));
    println!("loopback {:?}", loopback);
    // option used in place of null
    // T works like c# or C++
    let absent_number: Option<i32> = None;
    println!("absent_number {:?}", absent_number);
    let absent_number = Some(3);
    println!("absent_number {:?}", absent_number);
    println!("absent_number {:?}", absent_number.unwrap());
    

    //6.2
    let coin = Coin::Penny;
    println!("{:?} value is {:?}", coin, value_in_cents(&coin));
    let coin = Coin::Quarter(UsState::Alabama);
    println!("{:?} value is {:?}", coin, value_in_cents(&coin));

    let five = Some(5);
    let six = plus_one(five);
    let none = plus_one(None);

    let some_u8_value = 0u8;
    match some_u8_value {
        1 => println!("one"),
        3 => println!("three"),
        5 => println!("five"),
        7 => println!("seven"),
        _ => (),    // matches everything
    }

    //6.3
    let some_u8_value = Some(0u8);
    if let Some(3) = some_u8_value {    // if let is like a short match, syntax sugar
        println!("three");
    }
}

#[derive(Debug)] 
enum UsState {
    Alabama,
    Alaska,
    // --snip--
}
#[derive(Debug)]
enum Coin {
    Penny,
    Nickel,
    Dime,
    Quarter(UsState),   // only this type of coin has a state
}

fn value_in_cents(coin: &Coin) -> u8 {
    match coin {
        Coin::Penny => {
            println!("Lucky penny!");
            1
        },
        Coin::Nickel => 5,
        Coin::Dime => 10,
        Coin::Quarter(state) => {   // now we have a state variable to use
            println!("State quarter from {:?}!", state);
            25
        }
    }
}
// restrict option from T to only i32
fn plus_one(x: Option<i32>) -> Option<i32> {
    match x {
        None => None,   // must be exhaustive- include all matches for x
        Some(i) => Some(i + 1),
    }
}

pub fn remove_duplicates(s: String, k: i32) -> String {
    let mut sz = s;
    loop {
        let mut count = 1;
        let mut start = 0;
        let mut found = false;
        let mut last_c = 'X';
        for (i, c) in sz.chars().enumerate() {
            if c != last_c {
                count = 1;
                start = i;
                last_c = c;
            } else {
                count = count + 1;
                if count == k {
                    sz.replace_range(start..i + 1, "");
                    found = true;
                    break;
                }
            }
        }
        if !found {
            break sz;
        }
    }
}
fn add_offset(big_indexer: usize, delta: isize) -> Option<usize> {
    if delta < 0 {
        big_indexer.checked_sub(delta.wrapping_abs() as usize)
    } else {
        big_indexer.checked_add(delta as usize)
    }
}