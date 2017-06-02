import React from 'react'
import { Table, Button, message, Tag, Modal, Spin } from 'antd'
const { Column, ColumnGroup } = Table
const ButtonGroup = Button.Group

import styles from './PageDownload.scss'
import { deleteFile } from 'sources'

class PageUpload extends React.Component {
  state = {
    isLoading: true,
    isPreviewShow: false,
    isPreviewLoading: true,
    isPreviewFirstLoading: true,
    previewImgSrc: ''
  }
  
  handleDeleteClick = record => () => {
    this.setState({
      isLoading: true
    })
  
    deleteFile(record.id)
      .then(() => {
        return this.refreshData()
      })
      .then(() => {
        message.success(`删除成功！`)
      })
  }
  
  handlePreviewClick = record => () => {
    this.setState({
      previewImgSrc: `/api/files/${record.id}/${record.type.match(/^image\//) ? 'download' : 'preview'}`,
      isPreviewShow: true,
      isPreviewLoading: `/api/files/${record.id}/preview` !== this.state.previewImgSrc
    })
  }
  
  handlePreviewLoad = e => {
    this.setState({
      isPreviewLoading: false,
      isPreviewFirstLoading: false
    })
  }
  
  handleDownloadClick = record => e => {
    window.open(`/api/files/${record.id}/download`, record.fileName)
  }
  
  handleCancel = () => {
    this.setState({
      isPreviewShow: false
    })
  }
  
  refreshData = () => {
    this.props.refreshData().then(data => {
      this.setState({
        isLoading: false
      })
    })
  }
  
  componentDidMount() {
    this.refreshData()
  }
  
  render() {
    return (
      <div>
        <Table
          dataSource={this.props.fileData}
          loading={this.state.isLoading}
          className={styles.table}
          pagination={false}>
          <Column
            title="编号"
            dataIndex="id"
            key="id"
            width="65px"/>
          <Column
            title="文件名"
            key="fileName"
            dataIndex="fileName"
            render={(text, record) => (
              <span>
                {text}
                {record.hasPreviewImage ? <Tag className={styles.tag} color="blue">有文档预览</Tag> : null}
                {record.type.match(/^image\//) ? <Tag className={styles.tag} color="blue">有图片预览</Tag> : null}
              </span>
            )}/>
          <Column
            title="上传时间"
            dataIndex="uploadDate"
            key="uploadDate"
            width="135px"/>
          <Column
            title="MD5"
            dataIndex="md5"
            key="md5"/>
          <Column
            title="操作"
            key="action"
            width="150px"
            render={(text, record) => (
              <ButtonGroup>
                <Button
                  size="small"
                  disabled={!record.hasPreviewImage && !record.type.match(/^image\//)}
                  onClick={this.handlePreviewClick(record)}>
                  预览
                </Button>
                <Button
                  size="small"
                  onClick={this.handleDownloadClick(record)}>
                  下载
                </Button>
                <Button 
                  size="small"
                  onClick={this.handleDeleteClick(record)}>
                  删除
                </Button>
              </ButtonGroup>
            )}/>
        </Table>
        <Modal
          className={styles.modal}
          visible={this.state.isPreviewShow}
          onCancel={this.handleCancel}
          footer={null}>
          <div className={styles['preview-wrapper']}>
            <Spin
              spinning={this.state.isPreviewLoading}
              tip="加载中...">
              <img
                src={this.state.previewImgSrc}
                alt="预览图"
                onLoad={this.handlePreviewLoad}
                style={{
                  height: this.state.isPreviewFirstLoading ? '100px' : 'auto',
                  opacity: this.state.isPreviewLoading ? 0 : 1
                }}/>
            </Spin>
          </div>
        </Modal>
      </div>
    )
  }
}

export default PageUpload