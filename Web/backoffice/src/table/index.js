import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import { Icon, Table as _Table, Pagination, Set } from "bumbag"
import ReactTable, { useTable as _useTable } from "react-table"
import { fromIsDescending, equalsDescending } from "./order"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

const DefaultState = {
  pageIndex: 0,
  pageSize: 10,
  sortBy: []
}

export const useTable = (initialState = {}) => {
  let [state, set] = useState({ ...DefaultState, ...initialState })

  const setPageIndex = (pageIndex) => {
    set((prevState) => ({ ...prevState, pageIndex }))
  }

  const setPageSize = (pageSize) => {
    set((prevState) => ({
      ...prevState,
      pageSize
    }))
  }

  const setState = (state) => {
    set((prevState) => ({ ...prevState, ...state }))
  }

  const setSortOrder = (sortBy) => {
    set((prevState) => {
      return { ...prevState, sortBy }
    })
  }

  return {
    state,
    setState,
    setPageIndex,
    setPageSize,
    setSortOrder
  }
}

/*
 pageIndex -> page
 pageSize -> rows
*/
export const toApiParams = (state) => {
  if (!state) {
    return undefined
  }

  let { pageIndex, pageSize, sortBy = [], ...restState } = state
  let page = pageIndex + 1
  let rows = pageSize
  let [sort] = sortBy
  let sortby = ""
  let orderby = ""
  if (sort) {
    sortby = sort?.id
    orderby = fromIsDescending(sort?.desc)
  }
  return {
    ...restState,
    page,
    rows,
    sortby,
    orderby
  }
}

export const Table = ({
  columns,
  data = [],
  initialState,
  controlledState,
  useSortBy = ReactTable.useSortBy,
  manualSortBy = true,
  setSortOrder,
  disableMultiSort = true,
  usePagination = ReactTable.usePagination,
  pageCount,
  setPageIndex,
  manualPagination = true,
  useFilters = ReactTable.useFilters,
  manualFilters = true,
  isLoading,
  head = true,
  body = true,
  foot = false,
  children,
  ...props
}) => {
  let table = _useTable(
    {
      columns,
      data,
      initialState,
      useControlledState: (state) => {
        let pageIndex = controlledState?.pageIndex ?? state.pageIndex
        let pageSize = controlledState?.pageSize ?? state.pageSize

        return React.useMemo(
          () => ({
            ...state,
            pageIndex,
            pageSize
          }),
          [JSON.stringify(state), pageIndex]
        )
      },
      manualSortBy,
      disableMultiSort,
      manualPagination,
      pageCount,
      manualFilters
    },
    useFilters,
    useSortBy,
    usePagination
  )

  let { getTableProps, gotoPage, state: tableState } = table
  let { pageIndex } = tableState

  useEffect(() => {
    setSortOrder && setSortOrder(tableState?.sortBy)
  }, [JSON.stringify(tableState?.sortBy)])

  const getPage = () => pageIndex + 1
  return (
    <>
      <Set orientation="vertical">
        <_Table width="100%" overflow="auto" {...getTableProps()} {...props}>
          {head === true && <Head table={table} />}
          {body === true && <Body table={table} />}
          {children}
          {foot === true && <Foot table={table} />}
        </_Table>
        {usePagination && pageCount > 0 && (
          <Pagination
            currentPage={getPage()}
            onChangePage={(page) => {
              let pageIndex = page - 1
              setPageIndex(pageIndex)

              gotoPage(pageIndex)
            }}
            numberOfPages={pageCount}
            css={css`
              z-index: 0;
            `}
          />
        )}
      </Set>
      {isLoading && <div>loading</div>}
    </>
  )
}

const Head = ({ table }) => {
  let { headerGroups } = table
  return (
    <_Table.Head>
      {headerGroups.map((headerGroup) => (
        <_Table.Row {...headerGroup.getHeaderGroupProps()}>
          {headerGroup.headers.map((column) => {
            let props = column.getHeaderProps({
              ...column?.headCell,
              ...column?.getSortByToggleProps()
            })
            return (
              <_Table.HeadCell {...props}>
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
              </_Table.HeadCell>
            )
          })}
        </_Table.Row>
      ))}
    </_Table.Head>
  )
}

const Foot = ({ table }) => {
  let { footerGroups } = table
  return (
    <_Table.Foot fontWeight="semibold">
      {footerGroups.map((group) => (
        <_Table.Row {...group.getFooterGroupProps()}>
          {group.headers.map((column) => (
            <_Table.Cell
              {...column.getFooterProps({ ...column?.footerCellProps })}
            >
              {column.render("Footer")}
            </_Table.Cell>
          ))}
        </_Table.Row>
      ))}
    </_Table.Foot>
  )
}

const Body = ({ table }) => {
  let { rows, getTableBodyProps, prepareRow } = table

  return (
    <_Table.Body {...getTableBodyProps()}>
      {rows.map((row, i) => {
        prepareRow(row)
        return (
          <_Table.Row {...row.getRowProps()}>
            {row.cells.map((cell) => {
              let props = cell.getCellProps({ ...cell?.column?.cell })
              return <_Table.Cell {...props}>{cell.render("Cell")}</_Table.Cell>
            })}
          </_Table.Row>
        )
      })}
    </_Table.Body>
  )
}

export const pageCount = (pageSize, totalCount) => {
  if (!pageSize) {
    return
  }

  let numberOfPages = Math.ceil(totalCount / pageSize)
  return numberOfPages
}
