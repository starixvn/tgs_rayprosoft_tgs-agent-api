import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import { useAsync } from "react-async"
import {
  Heading,
  Box,
  PageContent,
  Container,
  Stack,
  Icon,
  Table,
  Pagination,
  Button,
  InputField,
  Set
} from "bumbag"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

const DefaultState = {
  page: 1,
  /* page size */
  rows: 10,
  sortby: "",
  orderby: "",
  search: [
    /* [field, value] */
  ],
  promise: null
}

export const useTable = ({ initialState = DefaultState, ...props } = {}) => {
  let [state = {}, set] = useState(initialState)

  let { page, rows, sortby, orderby, promise } = state
  return {
    page,
    setPage: (page) =>
      set((prevState) => {
        return { ...prevState, page }
      }),
    rows,
    setRows: (rows) =>
      set((prevState) => {
        return { ...prevState, rows }
      }),
    sortby,
    orderby,
    setSortOrder: ({ sortby = "", orderby = "" }) =>
      set((prevState) => {
        return { ...prevState, sortby, orderby }
      }),
    promise,
    setPromise: (promise) =>
      set((prevState) => {
        return { ...prevState, promise }
      })
  }
}

export default ({ table, children, ...props }) => {
  let { page, setPage, rows, promise } = table
  let { data = {}, ...async } = useAsync({
    promise
  })

  let { totalCount = 0 } = data
  let numberOfPages = Math.ceil(totalCount / rows)

  //TODO: support react children?
  return (
    <Set>
      {children({ data, table, async, ...props })}
      <Box>
        <Pagination
          currentPage={page}
          onChangePage={(page) => {
            setPage(page)
          }}
          numberOfPages={numberOfPages}
        />
      </Box>
    </Set>
  )
}
