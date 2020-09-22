import React, {
  useEffect,
  useMemo,
  useState,
  createContext,
  useContext
} from "react"
import { css, Global } from "@emotion/core"
import { Set, Button, Table, Pagination, Icon } from "bumbag"
import Async, { useAsync } from "react-async"
import { useTable, usePagination, useSortBy, useFilters } from "react-table"
import { fromIsDescending } from "./order"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

const Context = createContext({})

const defaultState = {
  pageIndex: 0,
  pageSize: 10,
  sortBy: [],
  request: Promise.resolve()
}

let HeadIndex = 0

export const useTableState = (value = defaultState) => {
  let [state, set] = useState({ ...defaultState, ...value })
  //let async = useAsync(request ?? Promise.resolve())

  const setState = (state) =>
    set((oldState) => ({
      ...oldState,
      ...state
    }))

  const headSortBy = () => {
    let { id = "" } = state?.sortBy?.[HeadIndex] ?? {}
    return id
  }

  const headOrderBy = () => {
    let sortBy = headSortBy()
    return sortBy && fromIsDescending(state?.sortBy?.[HeadIndex]?.desc)
  }

  return {
    state,
    setState,
    headSortBy,
    headOrderBy
  }
}

export default ({
  promise,
  mapDataItems = defaultMapDataItems,
  mapNumberOfPages = defaultMapNumberOfPages,
  state,
  onStateChange,
  head,
  columns,
  body,
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
      manualSortBy: true,
      disableMultiSort: true,
      manualPagination: true,
      manualFilters: true,
      pageCount: mapNumberOfPages({ data, pageSize: state?.pageSize })
    },
    useFilters,
    useSortBy,
    usePagination
  )
  let { getTableProps, rows, gotoPage, state: tableState } = table
  let { pageIndex, pageSize, sortBy } = tableState

  const getPage = () => pageIndex + 1

  useEffect(() => {
    onStateChange({ pageIndex, pageSize, sortBy })
  }, [pageIndex, pageSize, sortBy])

  let tableProps = getTableProps()
  return (
    <Context.Provider value={{ table, async, getPage }}>
      <Set orientation="vertical">
        <Table width="100%" overflowX="auto" {...tableProps} {...props}>
          {head === true && <Head table={table} async={async} />}
          {body === true && <Body table={table} async={async} />}
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

const Head = ({ table }) => {
  let { headerGroups } = table
  return (
    <Table.Head>
      {headerGroups.map((headerGroup) => (
        <Table.Row {...headerGroup.getHeaderGroupProps()}>
          {headerGroup.headers.map((column) => {
            let props = column.getHeaderProps({
              ...column?.headCell,
              ...column?.getSortByToggleProps()
            })
            return (
              <Table.HeadCell {...props}>
                {column.render("Header")}
                {column.isSorted ? (
                  column.isSortedDesc ? (
                    <Icon icon={faCaretDown} type="font-awesome" />
                  ) : (
                    <Icon icon={faCaretUp} type="font-awesome" />
                  )
                ) : (
                  ""
                )}
              </Table.HeadCell>
            )
          })}
        </Table.Row>
      ))}
    </Table.Head>
  )
}

const Body = ({ table }) => {
  //let { isPending } = async
  let { rows, getTableBodyProps, prepareRow } = table

  return (
    <Table.Body {...getTableBodyProps()}>
      {rows.map((row, i) => {
        prepareRow(row)
        return (
          <Table.Row {...row.getRowProps()}>
            {row.cells.map((cell) => {
              let props = cell.getCellProps({ ...cell?.column?.cell })
              return <Table.Cell {...props}>{cell.render("Cell")}</Table.Cell>
            })}
          </Table.Row>
        )
      })}
    </Table.Body>
  )
}

const defaultMapDataItems = (data = {}) => {
  console.log("defaultMapDataItems", data.items)
  return data?.items ?? []
}

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
