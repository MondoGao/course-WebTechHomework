let proxy = {
  '/api': {
    target: "http://localhost:11379",
    changeOrigin: true
  }
}

if (process.env.NODE_ENV === 'develop') {

}

module.exports = {
  proxy
}