import React from 'react'
import { Upload, Button, Icon, Card, message, Badge, Modal, Spin } from 'antd'
const Dragger = Upload.Dragger

import styles from './PageUpload.scss'
import { postOrder } from 'sources'

/**
 * 要生成预览的 Mime Type
 * @type {[string]}
 */
const previewFileType = [
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'application/msword',
  'application/vnd.ms-powerpoint',
  'application/vnd.ms-excel',
  'application/pdf'
]

class PageUpload extends React.Component {
  state = {
    fileList: [],
    orderInfo: null,
    isGeneratingOrder: false
  }
  
  /**
   * 控制文件列表变化
   * @param fileInfo
   */
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
  
  /**
   * 获取上传成功的文件列表
   * @return {Array}
   */
  getSuccessFileList = () => {
    return this.state.fileList.filter(file => file.status === 'done')
  }
  
  /**
   * 处理订单生成按钮单击
   * @param e
   */
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
  
  /**
   * 处理模态框关闭事件
   */
  handleModelClose = () => {
    this.setState({
      fileList: [],
      orderInfo: null
    })
  }
  
  /**
   * 处理模态框复制信息按钮单击
   */
  handleModelCopy = () => {
    this.modelTip.select()
    document.execCommand('copy')
    message.success('复制成功！')
  }
  
  render() {
    const fileNum = this.getSuccessFileList().length
    const hasFile = fileNum > 0
  
    /**
     * 判断加载状态决定是否需要显示加载
     * @type {XML}
     */
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
  
    /**
     * 当获取到 Order 信息时才进行渲染
     */
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