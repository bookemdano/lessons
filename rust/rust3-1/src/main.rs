fn main() {
    // need mut keyword to easily change values
    let mut mutx = 5;
    println!("The value of x is: {}", mutx);
    mutx = 6;
    println!("The value of x is: {}", mutx);
    // can rewrite non-mut types with let
    let x = 0;
    let x = x + 1;
    let x = x * 2;
    println!("The value of x is: {}", x);
    // you can even change the type with let, creates a shadow variable
    let stuff = "fsaji9ofion";
    println!("The value of stuff is: {}", stuff);
    let stuff = stuff.len();
    println!("The value of stuff is: {}", stuff);

    // requires type because it can't guess
    let whatsit: u32 = "42".parse().expect("Not a number!");
    println!("The value of guess is: {}", whatsit);

    // emoji types- VSCode terminal can't handle displaying but PS can
    let c = '🙌';
    println!("The value of c is: {}", c);

    // overflow causes panic in debug, just rolls in prod
    let mut b: u8 = 0;
    loop 
    {
        b += 1;
        if b == 255
        {
            println!("The value of b is: {}", b);   // panic at 256!
            break;
        }
    }
    
    // tuples- fixed length, each member can have its own type
    let mut tup = (500, 6.4, 1);

    // prefix intentially unused vars with _
    let (_x, y, _z) = tup;

    println!("The value of y is: {}", y);
    println!("The value of tup.0 is: {}", tup.0);
    tup.0 = 99;
    println!("The value of tup.0 is: {}", tup.0);

    // arrays- fixed len, fixed type. stack instead of heap
    let _a = [1,2,4,5];
    let _a: [i32; 4] = [1,2,4,5];
    let a = ['a';4];  // a bunch of a's
    let _third = a[2];
    // let _xth = a[5]; compile time error but could happen in prod based on data so it panics

    another_function(5);

    // expressions
    let y = {
        let x = 3;
        x + 1   // note no semicolon means this is the return value
    };
    println!("The value of y is: {}", y); 

    println!("The value of addsome is: {}", add_some(5)); 

    // if and else
    let number = 3;
    if number < 5 {
        println!("condition was true");
    } else {
        println!("condition was false");
    }

    // since if is an expression- it can be used to assign
    let condition = true;
    let number = if condition { 5 } else { 6 }; // must be same type
    println!("The value of number is: {}", number);

    // loops are also expressions
    let mut counter = 0;
    let result = loop {
        counter += 1;

        if counter == 10 {
            break counter * 2;  // break takes a return value
        }
    };
    println!("The result is {}", result);

    // while
    let mut number = 3;
    while number != 0 {
        println!("{}!", number);
        number -= 1;
    }

    // foreachish
    let a = [10, 20, 31, 40, 50];
    for e in a.iter() {
        println!("the value is: {}", e);
    }

    // loop with range type from stdlib
    for number in (1..5).rev() {    // range is (included..excluded)
        println!("{}!", number);
    }

    // fib test
    let n = 15;
    let mut ith = 0;
    let mut fib = 0;
    let mut lastfib = 1;
    let result = loop {
        ith += 1;
        let oldfib = fib;
        fib += lastfib;
        lastfib = oldfib;
        println!("The {}th fib is {}!", ith, fib);
        if ith >= n {
            break fib;
        }
    };
    println!("The {}th fib is {}!", n, result);

}

fn fib(n: i32) -> i32 {
    match n {
        0 => 0,
        1 => 1,
        _ => {
            let mut ith = 0;
            let mut fib = 0;
            let mut lastfib = 1;
            
            let result = loop {
                ith += 1;
                let oldfib = fib;
                fib += lastfib;
                lastfib = oldfib;
                println!("The {}th fib is {}!", ith, fib);
                if ith >= n {
                    break fib;
                }
            };
            result
        },
    }
}

fn another_function(arg: i32) {
    println!("The value of arg is: {}", arg); 
}

fn add_some(arg: i32) -> i32 {
    arg + 5 // note no semicolon means this is the return value
}