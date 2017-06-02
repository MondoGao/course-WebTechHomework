import React from 'react'
import { Upload, Button, Icon, Card } from 'antd'

import styles from './PageUpload.scss'

class PageUpload extends React.Component {
  render() {
    return (
      <Card>
        <Upload
          name="uploadFile"
          action="/api/files">
          <Button>
            <Icon type="upload"/> upload
          </Button>
        </Upload>
      </Card>
    )
  }
}

export default PageUpload