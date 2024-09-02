use std::io;
use rand::Rng;
use std::cmp::Ordering;

fn main() {

    println!("Guess a number game!");
    
    let secret_number = rand::thread_rng().gen_range(1, 101);
    // println!("The secret number is: {}", secret_number);
    let mut roboguess = 50;
    let mut delta = roboguess / 2;
    let mut roboattempts = 0;
    loop {
        // println!("Roboguessed {} delta {}", roboguess, delta);
        roboattempts += 1;
        match roboguess.cmp(&secret_number) {
            Ordering::Less => {
                // too small
                roboguess += delta;
            },
            Ordering::Greater => roboguess -= delta,
            Ordering::Equal => {
                println!("You win!");
                break;
            },
        }
        delta = delta / 2;
        if delta < 1
        {
            delta = 1;
        }
    }
    println!("I guessed it in {} tries. Now you try.", roboattempts);
    let mut attempts = 0;
    loop {
        println!("Enter a number to guess");
        attempts += 1;
        let mut guess = String::new();
        
        io::stdin()
            .read_line(&mut guess)
            .expect("Failed to read line");

        let guess: u32 = match guess.trim().parse() {
            Ok(num) => num,
            Err(_) => {
                println!("NaN");
                continue;
            }
        };

        match guess.cmp(&secret_number) {
            Ordering::Less => println!("Too small!"),
            Ordering::Greater => println!("Too big!"),
            Ordering::Equal => {
                println!("You win!");
                break;
            },
        }
        // println!("You guessed: {}", guess);
    }

    match roboattempts.cmp(&attempts) {
        Ordering::Less => println!("I guessed faster!"),
        Ordering::Greater => println!("You guessed faster!"),
        Ordering::Equal => println!("Tie!"),
    }
}
