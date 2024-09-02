fn main() {
    let s1 = String::from("hello");
    let mut s2 = s1.clone();

    
    let l = calc_size_by_ref(&s1);
    println!("len1 {}", l);


    let (mut s1, l) = takes_ownership(s1);
    println!("len {}", l);
    {
        println!("s1 {}, s2 {}", s1, s2);
        s1.push_str(", world!"); // push_str() appends a literal to a String
    }
    let r1 = &mut s2;
    change(r1);

    change(&mut s2);
    println!("s1 {}, s2 {}", s1, s2);

    let first = find_between("if I were a rich man", ' ',  ' ');
    println!("first -{}-", first);
    let second = find_between("Houston, we have a problem here!", ',',  '!');
    println!("second -{}-", second);
    // let third = find_between("We have nothing to fear 🦔 but fear itself", '🦔',  '!');
    let third = find_between("We have nothing to fear ; but fear itself", ';',  '!');
    println!("third -{}-", third);

    let fourth = find_between("We have nothing to fear but fear itself", ';',  '!');
    println!("fourth -{}-", fourth);

}

fn takes_ownership(sz: String) -> (String, usize) {
    println!("inside {}", sz);
    let len = sz.len();
    (sz, len)
}
fn calc_size_by_ref(sz: &String) -> usize {
    println!("inside {}", sz);
    sz.len()
}

fn change(sz: &mut String) {
    sz.push_str(", somewhere");
}
fn new_string() -> String {
    let sz = String::from("over the rainbow");
    sz
}
fn find_between(sz: &str, start_tag: char, end_tag: char) -> &str
{
    let chars = sz.chars();
    let mut start_i= usize::MAX;
    let mut end_i = usize::MAX;
    for (i, item) in chars.enumerate() {
        if start_i == usize::MAX {
            if item == start_tag {
                println!("Found start {}", i);
                start_i = i + 1;    // don't include tag
            }
            continue;
        } else {
            if item == end_tag {
                println!("Found end {}", i);
                end_i = i;
                break;
            }
        }
    }
    
    if start_i == usize::MAX {
        &sz[0..0]
    } else if end_i == usize::MAX {
        &sz[start_i..]
    } else {
        &sz[start_i..end_i]
    }
}