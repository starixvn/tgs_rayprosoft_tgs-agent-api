import React, {
  useEffect,
  useMemo,
  useState,
  createContext,
  useContext
} from "react"
import { css, Global } from "@emotion/core"
import { Set, Button, Table, Pagination } from "bumbag"
import Async, { useAsync } from "react-async"
import { useTable, useFlexLayout, usePagination, useSortBy } from "react-table"

const Context = createContext({})

export default ({
  promise,
  mapDataItems = defaultMapDataItems,
  mapNumberOfPages = defaultMapNumberOfPages,
  state,
  onStateChange,
  fetchData,
  columns,
  children,
  ...props
}) => {
  let async = useAsync({ promise })
  let { data, error, isPending } = async

  let table = useTable(
    {
      columns,
      data: mapDataItems(data) ?? [],
      initialState: state,
      manualPagination: true,
      pageCount: mapNumberOfPages({ data, pageSize: state?.pageSize })
    },
    /*
    useSortBy,
    */
    useFlexLayout,
    usePagination
  )
  let { getTableProps, rows, gotoPage, state: tableState } = table
  let { pageIndex, pageSize } = tableState

  const getPage = () => pageIndex + 1

  useEffect(() => {
    onStateChange({ ...tableState })
    fetchData()
  }, [pageIndex, pageSize])

  //TODO: isPending
  return (
    <Context.Provider value={{ table, async, getPage }}>
      <Set orientation="vertical">
        <Table {...getTableProps()} {...props}>
          {children}
        </Table>
        {mapNumberOfPages && (
          <Pagination
            currentPage={getPage()}
            onChangePage={(page) => {
              let pageIndex = page - 1
              gotoPage(pageIndex)
            }}
            numberOfPages={mapNumberOfPages({ data, pageSize })}
          />
        )}
      </Set>
      {isPending && <div>TODO: loading indicator</div>}
    </Context.Provider>
  )
}

const defaultMapDataItems = (data = {}) => data?.items ?? []

//TODO: check page size number?
const DefaultPageSize = 10
const defaultMapNumberOfPages = ({
  data = {},
  pageSize = DefaultPageSize
} = {}) => {
  let { totalCount = 0 } = data
  let numberOfPages = Math.ceil(totalCount / pageSize)
  return numberOfPages
}

export const useAsyncTable = () => {
  return useContext(Context)
}

export const useIsPending = () => {
  let { async = {} } = useContext(Context)
  let { isPending = false } = async
  return isPending
}
