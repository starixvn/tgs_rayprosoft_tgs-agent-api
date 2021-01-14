export const allCheckedKeys = (keyCheckedPairs = {}) =>
  Object.entries(keyCheckedPairs).reduce((list, [key, checked]) => {
    if (checked) {
      return [...list, key]
    }

    return list
  }, [])
