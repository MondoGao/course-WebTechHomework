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