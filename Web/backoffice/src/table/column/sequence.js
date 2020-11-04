import React from "react"
import { css } from "@emotion/core"
import { Text } from "bumbag"
import { Trans } from "../../i18n"

export default () => {
  return {
    id: "sequence",
    accessor: (row, rowIndex) => {
      return {
        rowIndex
      }
    },
    Header: <Trans zh="序" en="Sequence" />,
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
