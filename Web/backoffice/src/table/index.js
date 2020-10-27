import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import { Icon, Table as _Table, Pagination, Set } from "bumbag"
import {
  useTable as _useTable,
  usePagination,
  useSortBy,
  useFilters
} from "react-table"
import { fromIsDescending } from "./order"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

const DefaultState = {
  pageIndex: 0,
  pageSize: 10,
  sortby: "",
  orderby: ""
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

  const setSortOrder = ({ sortby = "", orderby = "" }) => {
    set((prevState) => ({ ...prevState, sortby, orderby }))
  }

  return {
    state,
    setState,
    setPageIndex,
    setPageSize,
    setSortOrder
  }
}

export const Table = ({
  columns,
  data = [],
  controlledState = {},
  manualSortBy = true,
  setSortOrder,
  disableMultiSort = true,
  manualPagination = true,
  pageCount,
  manualFilters = true,
  setPageIndex,
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
      useControlledState: (state) => {
        let pageIndex = controlledState?.pageIndex ?? state.pageIndex
        let pageSize = controlledState?.pageSize ?? state.pageSize

        return React.useMemo(
          () => ({
            ...state,
            pageIndex,
            pageSize
          }),
          [state, pageIndex, pageSize]
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

  let HeadIndex = 0
  let sortby = tableState?.sortBy?.[HeadIndex]?.id ?? ""
  let orderby =
    sortby && fromIsDescending(tableState?.sortBy?.[HeadIndex]?.desc)

  useEffect(() => {
    setSortOrder && setSortOrder({ sortby, orderby })
  }, [sortby, orderby])

  const getPage = () => pageIndex + 1
  return (
    <>
      <Set orientation="vertical">
        <_Table width="100%" overflowX="auto" {...getTableProps()} {...props}>
          {head === true && <Head table={table} />}
          {body === true && <Body table={table} />}
          {children}
          {foot === true && <Foot table={table} />}
        </_Table>
        {pageCount > 0 && (
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
