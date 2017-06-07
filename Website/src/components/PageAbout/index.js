import React from 'react'
import { Card, Col, Row } from 'antd'

import styles from './PageAbout.scss'

class PageUpload extends React.Component {
  render() {
    const data = [
      {
        uid: 'U201516060',
        name: '江高华',
        class: '信管 1501',
        avatar: require('assets/jgh.gif'),
        incharge: '技术总监 & 前端'
      },
      {
        uid: 'U201516083',
        name: '朱芝',
        class: '信管 1502',
        avatar: require('assets/zz.gif'),
        incharge: '后端助理'
      },
      {
        uid: 'U201516084',
        name: '杨司玥',
        class: '信管 1502',
        avatar: require('assets/ysy.gif'),
        incharge: '后端助理'
      },
      {
        uid: 'U201515974',
        name: '唐清清',
        class: '信管 1502',
        avatar: require('assets/tqq.gif'),
        incharge: '数据库设计助理'
      },
      {
        uid: 'U201516080',
        name: '张胜男',
        class: '信管 1502',
        avatar: require('assets/zsn.gif'),
        incharge: '文档编写助理'
      }
    ]
    
    return (
      <Row type="flex" className={styles['row']}>
        {data.map(d => <Peer data={d} key={d.uid}/>)}
      </Row>
    )
  }
}

/**
 * 单个人员的信息组件
 * @param data
 * @return {XML}
 * @constructor
 */
const Peer = ({ data }) => {
  const colSetting = {
    xs: 12,
    md: 8,
    lg: 6,
    xl: 6,
    style: {
      padding: '20px'
    }
  }
  
  return (
    <Col {...colSetting}>
      <Card className={styles.card}>
        <div className={styles['avatar-wrapper']}>
          <img src={data.avatar} alt={data.name}/>
        </div>
        <h3>
          {data.name}
        </h3>
        <p>
          {data.incharge}
        </p>
        <p>
          {data.uid}
        </p>
        <p>
          {data.class}
        </p>
      </Card>
    </Col>
  )
}

export default PageUpload