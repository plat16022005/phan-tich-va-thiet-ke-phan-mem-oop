const express = require("express");
const axios = require("axios");
const jwt = require("jsonwebtoken");
const qs = require("querystring");

const app = express();

const CLIENT_ID =
  "796867223192-dtvudrhoemvbpsnoqgeoummirloholrt.apps.googleusercontent.com";
const CLIENT_SECRET = "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";
const REDIRECT_URI = "http://localhost:8080/";

app.get("/google-login", async (req, res) => {
  console.log("➡️ /google-login called");

  const code = req.query.code;
  if (!code) {
    return res.status(400).json({ error: "Missing code" });
  }

  try {
    console.log("➡️ Sending code to Google...");

    const tokenRes = await axios.post(
      "https://oauth2.googleapis.com/token",
      qs.stringify({
        code: code,
        client_id: CLIENT_ID,
        client_secret: CLIENT_SECRET,
        redirect_uri: REDIRECT_URI,
        grant_type: "authorization_code",
      }),
      {
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
        },
        timeout: 10000, // 🔥 rất quan trọng
      }
    );

    console.log("⬅️ Google responded");

    const idToken = tokenRes.data.id_token;
    const payload = jwt.decode(idToken);

    res.json({
      sub: payload.sub,        // 🔥 THÊM DÒNG NÀY
      email: payload.email,
      token: idToken,
    });
  } catch (err) {
    console.error("❌ GOOGLE ERROR");
    console.error(err.response?.data || err.message);

    res.status(500).json({
      error: "Google token exchange failed",
    });
  }
});

app.listen(3000, () => {
  console.log("✅ Backend running at http://localhost:3000");
});
