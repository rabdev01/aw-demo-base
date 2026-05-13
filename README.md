# AdventureWorks Demo — Dev Container Setup

An ASP.NET Core 10 MVC application backed by the **AdventureWorksLT 2019** sample database, running fully inside a VS Code Dev Container. No local .NET SDK or SQL Server installation required.

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Docker Engine in WSL](#windows--docker-engine-in-wsl-no-docker-desktop) | Latest | Must be running before opening the container |
| [VS Code](https://code.visualstudio.com/) | Latest | |
| [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) | Latest | `ms-vscode-remote.remote-containers` |
| Git | Any | For cloning the repo |

> **Windows users:** WSL 2 is required. You can run containers via **Docker Desktop** (GUI, easier setup) or **Docker Engine directly inside WSL** (no licence requirements, lighter weight). Both options are covered below.

---

## Windows — WSL 2 Setup

WSL 2 provides the Linux kernel that Docker Desktop relies on for container performance on Windows.

### 1. Enable WSL 2

Open **PowerShell as Administrator** and run:

```powershell
wsl --install
```

This installs WSL 2 and Ubuntu by default. Restart your machine when prompted.

If WSL was already installed but on version 1, upgrade it:

```powershell
wsl --set-default-version 2
```

### 2. Verify WSL 2 is active

```powershell
wsl --list --verbose
```

The `VERSION` column should show `2` for your distribution.

### 3. Configure Docker Desktop for WSL 2

1. Open Docker Desktop → **Settings** → **General**
2. Ensure **Use the WSL 2 based engine** is checked
3. Go to **Settings** → **Resources** → **WSL Integration**
4. Enable integration for your Ubuntu distribution
5. Click **Apply & Restart**

### 4. Clone the repo inside WSL (recommended)

For best I/O performance, clone the repository into the WSL filesystem rather than a Windows path:

```bash
# Inside your WSL terminal (Ubuntu)
cd ~
git clone <repo-url>
code .
```

> Cloning under `/mnt/c/...` (a Windows path) works but causes significantly slower container I/O.

---

## Windows — Docker Engine in WSL (no Docker Desktop)

If you prefer not to use Docker Desktop (e.g. to avoid licence requirements), you can install Docker Engine directly inside your WSL 2 distribution.

### 1. Complete WSL 2 setup first

Follow the [WSL 2 Setup](#windows--wsl-2-setup) section above through step 2, then continue here.

### 2. Install Docker Engine inside WSL

Open your WSL terminal (Ubuntu) and run:

```bash
# Remove any old Docker packages
sudo apt-get remove -y docker docker-engine docker.io containerd runc 2>/dev/null || true

# Install prerequisites
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg lsb-release

# Add Docker's official GPG key
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg \
  | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

# Add the Docker apt repository
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
  https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" \
  | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker Engine and Compose plugin
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

### 3. Allow your user to run Docker without sudo

```bash
sudo usermod -aG docker $USER
```

Close and reopen your WSL terminal for the group change to take effect.

### 4. Start Docker automatically with WSL

WSL 2 does not run systemd by default on older Ubuntu versions. Start the daemon manually if needed:

```bash
sudo service docker start
```

To start it automatically on every WSL launch, add the following to `~/.bashrc` or `~/.profile`:

```bash
# Start Docker daemon if not running
if ! pgrep -x dockerd > /dev/null; then
  sudo service docker start > /dev/null 2>&1
fi
```

If your WSL distribution has systemd enabled (Ubuntu 22.04+ with WSL 0.67.6+), enable it as a service instead:

```bash
sudo systemctl enable docker
sudo systemctl start docker
```

### 5. Verify Docker is working

```bash
docker run --rm hello-world
```

### 6. Open your project from within WSL

Install the **Remote - WSL** extension (`ms-vscode-remote.remote-wsl`) in VS Code, then launch VS Code from inside WSL:

```bash
cd ~/your-repo
code .
```

VS Code will connect to WSL and the Dev Containers extension will automatically use the Docker Engine running there. No additional configuration is needed.

---

## Opening the Dev Container

1. Open the cloned folder in VS Code
2. VS Code will detect the `.devcontainer/` folder and prompt:
   **"Reopen in Container"** — click it

   If the prompt does not appear, open the Command Palette (`Ctrl+Shift+P`) and run:
   ```
   Dev Containers: Reopen in Container
   ```

3. VS Code will build two Docker containers:
   - **`app`** — .NET 10 development environment
   - **`db`** — Azure SQL Edge instance

4. On first launch the database container will automatically download and restore the **AdventureWorksLT2019** backup (~7 MB). This happens once; subsequent starts use the persisted Docker volume.

5. When the container is ready the status bar will show the container name.

---

## Running the Application

Open the integrated terminal (`` Ctrl+` ``) and run:

```bash
cd src/AdventureWorksWeb
dotnet run
```

The app will be available at `http://localhost:5174`.

---

## Database Connection

The SQL Server instance is accessible inside the container at:

| Setting | Value |
|---------|-------|
| Server | `db,1433` |
| Authentication | SQL Login |
| Username | `SA` |
| Password | `P@ssw0rd123!` |
| Database | `AdventureWorksLT2019` |

The **SQL Server (mssql)** VS Code extension is pre-installed. Add a connection using the values above to browse the schema or run queries directly in the editor.

---

## Rebuilding the Container

If you change any `.devcontainer/` files or need a clean environment:

```
Dev Containers: Rebuild Container
```

To also wipe the database volume and start completely fresh:

```bash
docker compose -f .devcontainer/docker-compose.yml down -v
```

Then reopen in container.
