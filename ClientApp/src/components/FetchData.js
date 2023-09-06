import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;
    constructor(props) {
      super(props);
      this.state = {value: ''};
  
      this.handleChange = this.handleChange.bind(this);
      this.handleSubmit = this.handleSubmit.bind(this);
    }
  
    handleChange(event) {
      this.setState({value: event.target.value});
    }
  
    handleSubmit(event) {
      console.log("Me: " + this.state.value)
      fetch('weatherforecast/post',{
        method: 'POST',
        headers:{'Content-type':'application/json'},
        body: JSON.stringify({ textString: this.state.value})
      }).then(r=>r.json()).then(res=>{
        if(res){
          console.log("Assistant: " + res.textString)
        }
      });
      event.preventDefault();
    }
    
  render() {
    return (
      <div>
        <h1 id="tableLabel">Weather forecast</h1>

        <form onSubmit={this.handleSubmit}>
        <label>
          Name:
          <input type="text" value={this.state.value} onChange={this.handleChange} />
        </label>
        <input type="submit" value="Submit" />
      </form>
        
      </div>
    );
  }
}
