fn main() {
    println!("Hello 8, world!");
    // COLLECTIONS
    
    // 8.1 Vectors
    let v2 = vec![1,2,3];    // note that we don't need to declare type
    let mut v: Vec<i32> = Vec::new();
    v.push(10);
    v.push(20);
    v.push(30);
    v.push(40);
    v.push(50);

    let third: &i32 = &v[2];
    println!("The third element is {}", third);

    match v.get(2) {
        Some(third) => println!("The third element matches {}", third),
        None => println!("There is no third element."),
    }
    
    match v.get(9) {
        Some(third) => println!("The ninth element is {}", third),
        None => println!("There is no ninth element."),
    }
    
    let does_not_exist = v.get(100);    // just returns none
    println!("{:?}", does_not_exist);   
    //let does_not_exist = &v[100];     // panics

    let first = &v[0];
    //v.push(6);     // can't add to vector while holding any references to it

    // iterate
    for i in &v {
        println!("iterate {}", i);
    }
    // modify
    for i in &mut v {  
        *i += 2;
        println!("iterate {}", i);
    }


    // use enums for different types in the same vector
    #[derive(Debug)]
    enum SpreadsheetCell {
        Int(i32),
        Float(f64),
        Text(String),
    }

    let row = vec![
        SpreadsheetCell::Int(3),
        SpreadsheetCell::Text(String::from("azure")),
        SpreadsheetCell::Float(10.12),
    ];
    for i in &row {  
        println!("iterate {:?}", i);
    }

    println!("8.2 Strings");
    let mut s = String::new();
    // s = "contents"; not the right type
    println!("{}", s);
    s = String::from("contents");   // looks like this is prefered for some reason
    println!("{}", s);
    s = "contents".to_string();
    println!("{}", s);
    s = 5.to_string();
    println!("{}", s);
    s = String::from("你好");
    println!("{}", s);
    s = String::from("🦔");
    println!("{}", s);
    s.push_str("🦡");
    println!("{}", s);
    s.push('⛳');
    println!("{}", s);

    let s1 = String::from("s1 ");
    let s2 = String::from("s2 ");
    let s3 = s1 + &s2; // s1 is borrowed and returns as s3 for efficency
    //println!("{}", s1);   // s1 is borrowed because + really calls fn add(self, s: &str)
    println!("{}", s2);
    println!("{}", s3);
    
    let s4 = format!("{}-{}", s2, s3);  // can use like println! format- no ownership
    
    // instead of indexing, slice
    let hello = "дравствуйте";
    let s = &hello[0..4];  // will get 2 "graphemes(aka letters)", must be on grapheme boundaries or panic
    println!("{}", s); 

    //iterate by character- not some characters (3 and 5) are diacritics, which don't make sense on their own
    for c in "नमस्ते".chars() {
        println!("char {}", c);
    }
    // or bytes if you want it
    for b in "नमस्ते".bytes() {
        println!("byte {}", b);
    }

    println!("***8.3 Hash maps***");
    use std::collections::HashMap;  // use can be anywhere before it is needed
    let mut scores = HashMap::new();

    // string ownership is moved, int ownership is copied
    scores.insert(String::from("Blue"), 10);
    scores.insert(String::from("Yellow"), 50);
    println!("blue score {:?}", scores.get("Blue"));
    println!("red score {:?}", scores.get("Red"));
    
    // iterate
    for (key, value) in &scores {
        println!("{}: {}", key, value);
    }
    // or_insert
    scores.entry(String::from("Blue")).or_insert(50);   // do nothing if already exists
    println!("blue score or_insert {:?}", scores.get("Blue"));

    // or_insert
    scores.entry(String::from("Blue")).or_insert(50);   // do nothing if already exists
    println!("blue score or_insert {:?}", scores.get("Blue"));

    let teams = vec![String::from("Blue"), String::from("Yellow")];
    let initial_scores = vec![10, 50];

    let mut scores: HashMap<_, _> = // great name- zip  // _ infers type, we could put in <String, i32>
        teams.into_iter().zip(initial_scores.into_iter()).collect();
    

    // creating a new hash map
    let text = "hello world wonderful world";

    let mut map = HashMap::new();

    for word in text.split_whitespace() {
        let count = map.entry(word).or_insert(0);   // instead of awkward !contains()
        *count += 1;
    }

    println!("Map {:?}", map);
}
