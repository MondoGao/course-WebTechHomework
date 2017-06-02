import React from 'react';
import { Layout, Menu, Icon} from 'antd'
const { Header, Content, Footer, Sider } = Layout
const { Item } = Menu

import styles from './App.scss';

import {
  Route,
  Redirect
} from 'react-router-dom';

class App extends React.Component {
  state = {
    collapsed: false,
    mode: 'inline',
  }
  
  handleCollapse = collapsed => {
    console.log(collapsed)
    this.setState({
      collapsed,
      mode: collapsed ? 'vertical' : 'inline',
    })
  }
  
  render() {
    return (
      <Layout
        className={styles.app}>
        <Sider
          collapsible
          collapsed={this.state.collapsed}
          onCollapse={this.handleCollapse}>
          <div className={styles.logo}>
            <h2>
              {this.state.collapsed ? 'CF' : 'CloudFile'}
            </h2>
          </div>
          <Menu
            theme="dark"
            mode={this.state.mode}
            defaultSelectedKeys={['upload']}
            className={styles.menu}>
            <Item key="upload">
              <span>
                <Icon type="upload" />
                {this.state.collapsed ? null :<span className="nav-text">上传文件</span>}
              </span>
            </Item>
            <Item key="download">
              <span>
                <Icon type="download" />
                {this.state.collapsed ? null :<span className="nav-text">下载文件</span>}
              </span>
            </Item>
            <Item>
              <span>
                <Icon type="smile" />
                {this.state.collapsed ? null :<span className="nav-text">关于我们</span>}
              </span>
            </Item>
          </Menu>
        </Sider>
        <Layout>
          <Content className={styles.content}>
            <div>
              Blank
            </div>
          </Content>
          <Footer className={styles.footer}>
            Made with ❤ by CloudFile Group
          </Footer>
        </Layout>
      </Layout>
    )
  }
}

export default App;