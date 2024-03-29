import React from 'react'
import { Table, Button, message, Tag, Modal, Spin, Input  } from 'antd'
import { Route, Switch, Redirect } from 'react-router-dom'

import styles from './PageDownload.scss'
import { getOrder } from 'sources'

import FilesTable from 'components/FilesTable'

class PageUpload extends React.Component {
  state = {
    idValue: '',
    pwValue: '',
    orderData: null,
    isLoading: false
  }
  
  /**
   * 处理 id input 值变化
   * @param e
   */
  handleIdChange = e => {
    let value = e.target.value.replace(/[^0-9]/g, '').slice(0, 15)
    if (e.target.value !== value) {
      message.warning('订单编号只能为 15 位以内整数哦')
    }
    
    this.setState({
      idValue: value === '' ? '' : parseInt(value)
    })
  }
  
  /**
   * 处理 password input 值变化
   * @param e
   */
  handlePwChange = e => {
    let value = e.target.value.replace(/\s+/g, '').slice(0, 4)
    if (e.target.value.length > 4) {
      message.warning('密码都是 4 位的哦')
    }
    else if (e.target.value !== value) {
      message.warning('密码不能含有空格哦')
    }
    
    this.setState({
      pwValue: value
    })
  }
  
  /**
   * 刷新订单信息
   * @param replace
   * @return {Promise}
   */
  refreshOrder = (replace = true) => {
    this.setState({
      isLoading: true
    })
    
    return getOrder(this.state.idValue, this.state.pwValue)
      .then(data => {
        this.setState({
          orderData: data
        })
        this.props.history[replace ? 'replace' : 'push'](`/download/${this.state.orderData.id}`)
      })
      .catch(err => {
        this.setState({
          isLoading: false
        })
        console.log(err)
        message.error('编号或密码输入错误，请重新输入')
      })
  }
  
  /**
   * 处理回车键按下
   * @param e
   */
  handleEnterPress = e => {
    if (this.state.idValue.toString().length > 0 && this.state.pwValue.length > 3) {
      this.refreshOrder(false)
    } else {
      message.error('编号或密码长度不够，请重新输入')
    }
  }
  
  componentWillReceiveProps(nextProps) {
    if (nextProps.match.isExact && !this.props.isExact) {
      this.setState({
        isLoading: false
      })
    }
  }
  
  render() {
    
    return (
      <Switch>
        <Route path="/download/:id" render={props => {
          if (this.state.orderData) {
            return (
              <FilesTable {...props} fileData={this.state.orderData.files} refreshData={this.refreshOrder}/>
            )
          }
  
          /**
           * 若无订单信息则直接跳转
           */
          return (
            <Redirect push to="/download"/>
          )
        }}/>
        
        <Route render={props => (
          <div className="section">
            <p className={styles['input-container']}>
              <Input size="large" addonBefore="编号" value={this.state.idValue} onChange={this.handleIdChange} onPressEnter={this.handleEnterPress}/>
            </p>
            <p className={styles['input-container']}>
              <Input size="large" addonBefore="密码" value={this.state.pwValue} onChange={this.handlePwChange} onPressEnter={this.handleEnterPress}/>
            </p>
            <p className={styles['btn-container']}>
              <Button
                onClick={() => this.refreshOrder(false)}
                size="large"
                type="primary"
                loading={this.state.isLoading}
                disabled={!(this.state.idValue.toString().length > 0 && this.state.pwValue.length > 3)}>
                获取文件
              </Button>
            </p>
          </div>
        )}/>
      </Switch>
    )
  }
}

export default PageUpload