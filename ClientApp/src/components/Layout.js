import React, { Component } from "react";
import { Container } from "reactstrap";
import { NavMenu } from "./NavMenu";

export class Layout extends Component {
  static displayName = Layout.name;

  render() {
    return (
      <div
        style={{
          height: "100vh",
          maxHeight: "100%",
          color: "white",
          overflowY: "auto",
        }}
      >
        {this.props.children}
      </div>
    );
  }
}
