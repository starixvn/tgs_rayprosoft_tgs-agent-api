import React from "react"
import { css } from "@emotion/core"
import { Text } from "bumbag"

export default () => {
  //TODO: Header i18n
  return {
    accessor: (row, rowIndex) => {
      return {
        rowIndex
      }
    },
    Header: "序",
    disableSortBy: true,
    Cell: ({ state = {}, value = {} } = {}) => {
      let { pageIndex, pageSize } = state
      let { rowIndex } = value
      let row = rowIndex + 1
      let sequence = pageIndex * pageSize + row
      return (
        <Text.Block
          css={css`
            white-space: nowrap;
          `}
        >
          {sequence}
        </Text.Block>
      )
    }
  }
}
