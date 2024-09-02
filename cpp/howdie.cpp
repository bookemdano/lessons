#include <iostream>
#include <vector>
#include <string>

using namespace std;

//https://code.visualstudio.com/docs/languages/cpp

int main()
{
    std::cout << "Bonjour Monde" << std::endl;
    vector<string> msg {"Bon", "C++", "Jovi", "from", "VS Code", "and the C++ extension!"};

    for (const string& word : msg)
    {
        cout << word << " ";
    }
    cout << endl;
}
