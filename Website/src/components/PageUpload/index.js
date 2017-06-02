import React from 'react'
import { Upload, Button, Icon, Card, message } from 'antd'
const Dragger = Upload.Dragger

import styles from './PageUpload.scss'

const previewFileType = [
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/msword',
  'application/vnd.ms-powerpoint'
]

class PageUpload extends React.Component {
  handleChange = fileInfo => {
    const file = fileInfo.file
    if (file.percent === 100 && file.status === 'uploading' && previewFileType.includes(file.type)) {
      message.info(`为 ${file.name} 生成预览图中...`)
    } else if(file.status === 'done') {
      message.success(`${file.name} 上传成功！`)
    }
  }
  
  render() {
    return (
      <div>
        <Dragger
          name='uploadFile'
          multiple={true}
          action='/api/files'
          onChange={this.handleChange}
          className={styles.dragger}>
          <p className='ant-upload-drag-icon'>
            <Icon type='cloud-upload-o'/>
          </p>
          <p className={styles['upload-text']}>
            拖动或点击此处上传文件
          </p>
          <p className={styles['upload-hint']}>
            支持一次性上传多个文件，可对 Word，Excel，Powerpoint 文档及普通图片生成预览
          </p>
        </Dragger>
      </div>
    )
  }
}

export default PageUpload