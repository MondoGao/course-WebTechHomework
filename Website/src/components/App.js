import React from 'react';
import { Layout, Menu, Icon} from 'antd'
const { Header, Content, Footer, Sider } = Layout
const { Item } = Menu
import {
  Route,
  Link,
  Switch
} from 'react-router-dom';

import styles from './App.scss';
import { getFiles } from 'sources'

import PageUpload from 'components/PageUpload'
import PageDownload from 'components/PageDownload'
import PageAbout from 'components/PageAbout'

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
    let tabName = location.pathname.replace(/^\//, '')
    tabName = tabName ? tabName : 'about'
    
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
            selectedKeys={[tabName]}
            className={styles.menu}>
            <Item key="upload">
              <Link to="/upload">
                <Icon type="upload" />
                {this.state.collapsed ? null :<span className="nav-text">上传文件</span>}
              </Link>
            </Item>
            <Item key="download">
              <Link to="/download">
                <Icon type="download" />
                {this.state.collapsed ? null :<span className="nav-text">下载文件</span>}
              </Link>
            </Item>
            <Item key="about">
              <Link to="/">
                <Icon type="smile" />
                {this.state.collapsed ? null :<span className="nav-text">关于我们</span>}
              </Link>
            </Item>
          </Menu>
        </Sider>
        <Layout>
          <Content className={styles.content}>
            <Switch>
              <Route
                exact={true}
                path="/"
                component={PageAbout}/>
              <Route
                path="/download"
                render={props => (
                  <PageDownload {...props}/>
                )}/>
              <Route
                path="/upload"
                component={PageUpload}/>
            </Switch>
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