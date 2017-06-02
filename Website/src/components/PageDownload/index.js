import React from 'react'
import { Table, Button } from 'antd'
const { Column, ColumnGroup } = Table
const ButtonGroup = Button.Group

import styles from './PageDownload.scss'

class PageUpload extends React.Component {
  state = {
    isLoading: true
  }
  
  componentDidMount() {
    this.props.refreshData().then(data => {
      this.setState({
        isLoading: false
      })
    })
  }
  
  render() {
    return (
      <div>
        <Table
          dataSource={this.props.fileData}
          loading={this.state.isLoading}>
          <Column
            title="编号"
            dataIndex="id"
            key="id"/>
          <Column
            title="文件名"
            dataIndex="fileName"
            key="fileName"/>
          <Column
            title="上传时间"
            dataIndex="uploadDate"
            key="uploadDate"/>
          <Column
            title="MD5"
            dataIndex="md5"
            key="md5"/>
          <Column
            title="操作"
            key="action"
            render={(text, record) => (
              <ButtonGroup>
                {record.hasPreviewImage ? <Button size="small">
                  预览
                </Button> : null}
                <Button size="small">
                  详情
                </Button>
              </ButtonGroup>
            )}/>
        </Table>
      </div>
    )
  }
}

export default PageUpload