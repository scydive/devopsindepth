import React, { Component, useState, useEffect, useRef } from "react";
import { Button, InputGroup } from "reactstrap";
import { FormControl, FormControlState } from "@mui/base/FormControl";
import { Input, inputClasses } from "@mui/base/Input";

interface Chat {
  me: string;
  assistant: string;
}

interface ChatLog {
  items: Chat[];
}

export const FetchData = () => {
  useEffect(() => {
    populateWeatherData();
  }, []);
  const [string, setString] = useState("");
  let defaultList: ChatLog = {
    items: [{ me: "", assistant: "" }],
  };
  const [listChat, setChat] = useState<ChatLog>(defaultList);

  async function handleSubmit(event) {
    event.preventDefault();
    console.log("Me: " + string);
    await fetch("weatherforecast/post", {
      method: "POST",
      headers: { "Content-type": "application/json" },
      body: JSON.stringify({ me: string, assistant: "" }),
    })
      .then((r) => {
        console.log("bot is thinking");
        r.json();
      })
      .then((res) => {
        if (res) {
          console.log("Assistant: " + res.assistant);
        }
      });

    await populateWeatherData();
  }
  async function populateWeatherData() {
    let { me, valueMe }: any = "";
    let { assistant, valueAss }: any = "";
    const response = await fetch("weatherforecast/returnchat");
    const data = await response.json();
    valueMe = data[data.length - 1].me;
    valueAss = data[data.length - 1].assistant;
    let { items }: ChatLog = { items: [{ me: valueMe, assistant: valueAss }] };
    setChat((prevChat) => ({
      ...prevChat,
      items: data.reverse(),
    }));
  }

  return (
    <div
      style={{
        display: "flex",
        maxHeight: "100vh",
        flexDirection: "column",
      }}
    >
      <div style={{ position: "sticky", top: 0, backgroundColor: "#434755" }}>
        <h1
          style={{ color: "white", padding: "10px 10px 10px 20px" }}
          id="tableLabel"
        >
          MoAI
        </h1>
      </div>
      <div
        style={{
          color: "white",
          position: "relative",
          display: "flex",
          flexDirection: "column",
          overflowY: "auto",
          height: "100vh",
        }}
      >
        <div>
          <div
            style={{
              display: "flex",
              flexDirection: "column-reverse",
              scrollPaddingBottom: "100%",
            }}
          >
            {listChat.items
              ? listChat.items.map((res, index) => (
                  <p
                    style={{
                      width: "100%",
                      color: "white",
                    }}
                    key={index}
                  >
                    <span
                      style={{
                        width: "100%",
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        background: "#353540",
                        padding: "20px",
                      }}
                    >
                      <div style={{ width: "500px" }}>
                        You:<br></br> <span>{res.me}</span>
                      </div>
                    </span>
                    <br />
                    <span
                      style={{
                        width: "100%",
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        background: "#434755",
                        padding: "20px",
                      }}
                    >
                      <div style={{ width: "500px" }}>
                        Assistant:<br></br> <span>{res.assistant}</span>
                      </div>
                    </span>
                  </p>
                ))
              : "rip"}
          </div>
        </div>
      </div>
      <div
        style={{
          position: "sticky",
          bottom: 0,
          display: "flex",
          justifyContent: "center",
          background:
            "linear-gradient(0deg, rgba(52,53,64,1) 0%, rgba(52,53,64,0.8099614845938375) 50%, rgba(52,53,64,0) 100%)",
          maxHeight: "10%",
          padding: "30px",
        }}
      >
        <form onSubmit={handleSubmit}>
          <label>
            <input
              style={{
                color: "white",
                borderRadius: "10px",
                border: "none",
                width: "500px",
                background: "#41404f",
                padding: "15px",
                boxShadow: "box-shadow: 1px 1px 37px -11px rgba(0,0,0,0.53)",
              }}
              id="review-text"
              onChange={(e) => setString(e.target.value)}
              placeholder="Send a message"
              value={string}
            />
            <input type="submit" value="Submit" hidden />
          </label>
        </form>
      </div>
    </div>
  );
};
