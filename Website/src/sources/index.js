import { checkStatus } from './utils'

export const getFiles = () => {
  return fetch('/api/files', {
    credentials: 'same-origin'
  })
    .then(checkStatus)
    .then(data => data.json())
}

export const deleteFile = id => {
  return fetch(`/api/files/${id}`, {
    method: 'DELETE',
    credentials: 'same-origin'
  })
    .then(checkStatus)
    .then(data => data.json())
}

export const getOrder = (id, password) => {
  return fetch(`/api/orders/${id}?password=${password}`, {
    credentials: 'same-origin'
  })
    .then(checkStatus)
    .then(data => data.json())
}

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