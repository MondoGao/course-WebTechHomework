import React from 'react'
import { Upload, Button, Icon, Card, message, Badge, Modal, Spin } from 'antd'
const Dragger = Upload.Dragger

import styles from './PageUpload.scss'
import { postOrder } from 'sources'

const previewFileType = [
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/msword',
  'application/vnd.ms-powerpoint'
]

class PageUpload extends React.Component {
  state = {
    fileList: [],
    orderInfo: null,
    isGeneratingOrder: false
  }
  
  handleChange = fileInfo => {
    const file = fileInfo.file
    const fileList = fileInfo.fileList
  
    if (previewFileType.includes(file.type) && file.status === 'uploading') {
      fileList.find(f => f.uid === file.uid).percent /= 2
      if (file.percent === 100) {
        message.info(`为 ${file.name} 生成预览图中...`)
      }
    }
  
    if(file.status === 'done') {
      message.success(`${file.name} 上传成功！`)
      fileList.find(f => f.uid === file.uid).percent  = 100
    }
    
    this.setState({
      fileList
    })
  }
  
  getSuccessFileList = () => {
    return this.state.fileList.filter(file => file.status === 'done')
  }
  
  handleSubmitClick = e => {
    this.setState({
      isGeneratingOrder: true
    })
    
    postOrder(this.getSuccessFileList().map(fileInfo => fileInfo.response.id))
      .then(data => {
        this.setState({
          orderInfo: data,
          isGeneratingOrder: false
        })
        message.success(`订单上传成功！`)
      })
  }
  
  handleModelClose = () => {
    this.setState({
      fileList: [],
      orderInfo: null
    })
  }
  
  handleModelCopy = () => {
    this.modelTip.select()
    document.execCommand('copy')
    message.success('复制成功！')
  }
  
  render() {
    const fileNum = this.getSuccessFileList().length
    const hasFile = fileNum > 0
    
    let uploadTip = (
      <div>
        <p className='ant-upload-drag-icon'>
          <Icon type='cloud-upload-o'/>
        </p>
        <p className={styles['upload-text']}>
          拖动或点击此处上传文件
        </p>
        <p className={styles['upload-hint']}>
          支持一次性上传多个文件，可对 Word，Excel，Powerpoint 文档及普通图片生成预览
        </p>
      </div>
    )
    if (this.state.isGeneratingOrder) {
      uploadTip = (
        <Spin className={styles['upload-spin-container']}>
          {uploadTip}
        </Spin>
      )
    }
    
    const modelEles = this.state.orderInfo ? (
      <div className={styles['model-text']}>
        <p>您的订单编号为：<span>{this.state.orderInfo.id}</span></p>
        <p>下载密码为：<span>{this.state.orderInfo.password}</span></p>
        <p>请牢记编号和密码以便下载</p>
        <input ref={el => this.modelTip = el}
               type="text"
               value={`CloudFile 订单编号：${this.state.orderInfo.id} 密码：${this.state.orderInfo.password}`}/>
      </div>
    ) : null
    
    return (
      <div className="section">
        <Dragger
          name='uploadFile'
          multiple={true}
          action='/api/files'
          fileList={this.state.fileList}
          onChange={this.handleChange}
          className={styles.dragger}
          disabled={this.state.isGeneratingOrder}>
          {uploadTip}
        </Dragger>
        <div className={styles['btn-wrapper']}>
          <Badge count={fileNum}>
            <Button
              type="primary"
              size="large"
              disabled={!hasFile}
              loading={this.state.isGeneratingOrder}
              onClick={this.handleSubmitClick}>
              生成列表
            </Button>
          </Badge>
          <Modal
            visible={!!this.state.orderInfo}
            title="列表生成成功"
            onCancel={this.handleModelClose}
            footer={[
              <Button key="copy" onClick={this.handleModelCopy}>复制信息</Button>,
              <Button key="close" onClick={this.handleModelClose}>关闭</Button>
            ]}>
            {modelEles}
          </Modal>
        </div>
      </div>
    )
  }
}

export default PageUpload