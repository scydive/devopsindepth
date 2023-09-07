import React, { Component, useState, useEffect, useRef } from "react";
import { Button, InputGroup } from "reactstrap";
import { FormControl, FormControlState } from "@mui/base/FormControl";
import { Input, inputClasses } from "@mui/base/Input";
import '../custom.css';

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
  const [thinking, setThinking] = useState("");
  const [isSubmitted, setSubmitted] = useState(false);


  let defaultList: ChatLog = {
    items: [],
  };
  const [listChat, setChat] = useState<ChatLog>(defaultList);

  async function handleSubmit(event) {
    event.preventDefault();
    setString("");
    setThinking("The bot is thinking...")
    await fetch("weatherforecast/post", {
      method: "POST",
      headers: { "Content-type": "application/json" },
      body: JSON.stringify({ me: string, assistant: "" }),
    })
      .then((r) => {
        r.json();
      })
      .then((res) => {
        if (res) {
          console.log(res);
        }
      });
      setSubmitted(true);

    await populateWeatherData();
  }
  async function populateWeatherData() {
    let { me, valueMe }: any = "";
    let { assistant, valueAss }: any = "";
    const response = await fetch("weatherforecast/returnchat");
    const data = await response.json();
    console.log(data);
    valueMe = data[data.length - 1].me;
    valueAss = data[data.length - 1].assistant;
    let { items }: ChatLog = { items: [{ me: valueMe, assistant: valueAss }] };
    setChat((prevChat) => ({
      ...prevChat,
      items: data.reverse(),
    }));
    setThinking("")
  }

  const useTypewriter = (text, speed = 50) => {
    const [displayText, setDisplayText] = useState('');
  
    useEffect(() => {
      let i = 0;
      const typingInterval = setInterval(() => {
        if (i < text.length) {
          setDisplayText(prevText => prevText + text.charAt(i));
          i++;
        } else {
          clearInterval(typingInterval);
        }
      }, speed);
  
      return () => {
        clearInterval(typingInterval);
      };
    }, [text, speed]);
  
    return displayText;
  };

  const Typewriter = ({ text, speed }) => {
    const displayText = useTypewriter(text, speed);
  
    return <p>{displayText}</p>;
  };

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
                  <div
                    style={{
                      width: "100%",
                      color: "white",
                    }}
                    key={index}
                  >
                    <div
                      style={{
                        width: "100%",
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        background: "#353540",
                        padding: "20px",
                      }}
                    >
                      <span style={{ width: "650px", letterSpacing: "0.5px" }}>
                        You:<br></br> <span>{res.me}</span>
                      </span>
                    </div>
                    <br />
                    <div
                      style={{
                        width: "100%",
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "center",
                        background: "#434755",
                        padding: "20px",
                      }}
                    >
                      <span style={{ width: "650px", letterSpacing: "0.5px" }}>
                        Assistant:<br></br> <pre style={{whiteSpace: 'pre-wrap'}}>{index == 0 && isSubmitted ? <Typewriter text={res.assistant} speed={10}></Typewriter> : res.assistant}</pre>
                      </span>
                    </div>
                  </div>
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
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          textAlign: "left",
          background:
            "linear-gradient(0deg, rgba(52,53,64,1) 0%, rgba(52,53,64,0.8099614845938375) 50%, rgba(52,53,64,0) 100%)",
          maxHeight: "10%",
          padding: "30px",
        }}
      >
        <p id="pulsate" style={{ width: '650px' }}>{thinking}</p>
        <form onSubmit={handleSubmit}>
          <label>
            <input
              style={{
                color: "white",
                borderRadius: "10px",
                border: "none",
                width: "650px",
                background: "#41404f",
                padding: "15px",
                boxShadow: "box-shadow: 1px 1px 37px -11px rgba(0,0,0,0.53)",
              }}
              id="review-text"
              onChange={(e) => (setString(e.target.value), setSubmitted(false))}
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
