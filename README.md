# interview-question-002
For interview with question test (.NET Core + Vue + SQLite)

**This repo is proof-of-concept only, don't use for production directly**

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (20+)

## How to Run

```bash
# 1. Clone the repo
git clone https://github.com/Doiinn/interview-question-002.git
cd interview-question-002

# 2. Start the backend (terminal 1)
cd backend
dotnet run
# Backend running at http://localhost:5173

# 3. Start the frontend (terminal 2)
cd frontend
npm install
npm run dev
# Frontend running at http://localhost:5178
```

The frontend dev server proxies `/api/*` requests to the backend, so no CORS setup is needed.