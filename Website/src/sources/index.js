import { checkStatus } from './utils'

/**
 * 删除特定文件
 * @param id
 * @return {Promise}
 */
export const deleteFile = id => {
  return fetch(`/api/files/${id}`, {
    method: 'DELETE',
    credentials: 'same-origin'
  })
    .then(checkStatus)
    .then(data => data.json())
}

/**
 * 获取制定 id 的订单详情
 * @param id - 订单编号
 * @param password - 密码
 * @return {Promise}
 */
export const getOrder = (id, password) => {
  return fetch(`/api/orders/${id}?password=${password}`, {
    credentials: 'same-origin'
  })
    .then(checkStatus)
    .then(data => data.json())
}

/**
 * 创建订单并获取编号及密码
 * @param fileIds - 文件 ID 数组
 * @return {Promise}
 */
export const postOrder = fileIds => {
  return fetch(`/api/orders`, {
    method: 'POST',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      files: fileIds
    })
  })
    .then(checkStatus)
    .then(data => data.json())
}