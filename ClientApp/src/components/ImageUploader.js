import React, { Component } from 'react';
import axios from 'axios';

export class ImageUploader extends Component {
    displayName = ImageUploader.name

  constructor(props) {
    super(props);
    this.state = { currentFiles: [], responseMessage: "" };
      this.fileHandler = this.fileHandler.bind(this);
      this.uploadHandler = this.uploadHandler.bind(this);
  }

    fileHandler(e) {
        //console.log(e.target.files[0]);
        this.setState({ currentFiles: e.target.files });
    }

    uploadHandler(e) {
        
        const form = new FormData();
        if (this.state.currentFiles[0]) {   
            form.append('image', this.state.currentFiles[0]);
            axios.post("https://localhost:44332/api/ImageUpload", form)
                .then(res => { this.setState({ responseMessage: res.data }) })
                .catch(err => { console.log(err) });
        }   
    }

  render() {
    return (
      <div>
            <h1>Upload Images</h1>

            <input type="file" onChange={this.fileHandler} />
            <button onClick={this.uploadHandler}>Upload</button>
            <div>
            <p>{this.state.responseMessage}</p>
            </div>

      </div>
    );
  }
}
