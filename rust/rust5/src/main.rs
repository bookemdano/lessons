#[derive(Debug)]
struct User {
    username: String,
    email: String,
    login_count: i32,
}
// can be one or multiple impl's
impl User {
    fn is_valid(&self) -> bool {
        self.login_count > 0
    }

    fn is_new(&self) -> bool {
        self.login_count == 0
    }
    // associated function, like a static
    fn fake_user() -> User {
        User {
            username: String::from("fake"),
            email: String::from("fake@b.com"),
            login_count: 0,       
        }
    }
}
// structs members don't need names, called tuple struct
struct Color(i32, i32, i32);    

fn main() {
    println!("Hello, world!");
    // whole instance must be mutable
    // note- no new
    let mut user1 = User {
        username: String::from("myname"),
        email: String::from("a@b.com"),
        login_count: 0,
    };
    // :? shows the debug info
    println!("User is {:?}", user1);
    println!("User is valid {}", user1.is_valid());

    // don't need name if variable is the same
    let username = String::from("someone");
    let mut user1 = User {
        username,
        email: String::from("a@b.com"),
        login_count: 0,
    };

    // fill in the rest of the fields with user1 value
    let user2 = User {
        login_count: 1,
        .. user1
    };

    user1.email = String::from("c@d.com");


}
