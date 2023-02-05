# Running Github Actions Locally using act Binary in Linux
Github Actions allow developers to automate their workflow and testing procedures. However, it is often beneficial to test Github Actions locally before committing them to Github. This can be done using the act binary.

In this guide, we will cover how to run Github Actions locally using the act binary in a Linux environment.

## Prerequisites
* Linux operating system
* Docker
* Github Personal Use Token
* `act` binary installed

## Steps
* Install Docker and run the daemon.
  * Install Docker, if you do not already have it:
    ```bash
    curl -s https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash;
    sudo mkdir -p /etc/apt/keyrings;
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg;
    sudo chmod a+r /etc/apt/keyrings/docker.gpg
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
    sudo apt update
    sudo apt install docker-ce docker-ce-cli containerd.io
    ```
  * Run the Docker Daemon in the background:
    ```bash
    sudo dockerd&
    ```
* Create a Github token to allow Act to access Github on your behalf
  * Log in to your Github account.
  * Go to the settings page by clicking on your avatar in the upper right corner and selecting "Settings."
  * On the left-side panel, click on "Developer settings."
  * Under "Developer settings," click on "Personal access tokens."
  * Click on "Generate new token" button.
  * Enter a descriptive name for the token (e.g. "CI Token").
  * Select the scopes you want the token to have access to. The scopes you select will depend on what actions you plan to perform with the token.
  * Click on "Generate token."
  * Copy the token to a secure location. This is the only time you will see the token, so if you lose it, you will need to create a new one.
  * **Note:** You should keep your personal access tokens secret, as they grant access to your Github account. Do not share or hardcode them in your code.
* Install and run "Act"
  * Download the act binary. This can be installed by running:
    ```bash
    curl -s https://raw.githubusercontent.com/nektos/act/master/install.sh | sudo bash
    ```
  * Save the binary to a location on your system where it can be easily accessed, such as ~/bin/.
  * Make the binary executable by running the following command:
    ```bash
    sudo chmod +x ~/bin/act
    ```
  * Navigate to your Github repository in the terminal.
  * Run the following command to run the Github Actions workflow locally:
    ```bash
    sudo ./bin/act -s GITHUB_TOKEN=XXX -P ubuntu-latest=nektos/act-environments-ubuntu:18.04
    ```
  * Replace `XXX` with your Github token. The `-P` option specifies the runner environment. In
  this case, ubuntu-latest is specified as `nektos/act-environments-ubuntu:18.04.`
    * **Note:** This image is over _10 GB_, but contains most of the necessary binaries to emulate the CI.
  * Observe the output in the terminal to determine if the Github Actions workflow ran successfully.

## Conclusion
With these steps, you should be able to run Github Actions locally using the act binary in a Linux environment. This can be a valuable tool for testing Github Actions workflows before committing them to Github.



