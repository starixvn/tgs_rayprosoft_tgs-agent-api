import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/core"
import { Box, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { DeleteButton } from "./delete"
import { Code } from "./menu"
import { useDepartmentList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

const AsyncContent = () => {
  let { state, setState, setPageIndex, setSortOrder } = useTable()

  let { pageIndex, pageSize, sortby, orderby } = state

  let { data, isLoading, mutate } = useDepartmentList(toApiParams(state))
  const fetchTable = mutate
  let columns = useTableColumns({ fetchTable })

  return (
    <PageContent
      title={<Trans zh="部门管理" en="Departments" />}
      actionBar={
        <Box alignX="right">
          <AddButton onSubmitted={fetchTable} />
        </Box>
      }
    >
      <Table
        controlledState={state}
        columns={columns}
        data={data?.items}
        pageCount={pageCount(pageSize, data?.totalCount)}
        setPageIndex={setPageIndex}
        setSortOrder={setSortOrder}
        isLoading={isLoading}
      />
    </PageContent>
  )
}

const useTableColumns = ({ fetchTable }) => {
  let columns = [
    sequenceColumn(),
    {
      Header: <Trans zh="部门名称" en="Name" />,
      id: "name",
      accessor: "name",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    }
  ]
  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      disableSortBy: true,
      accessor: (row) => {
        return {
          id: row.id,
          name: row.name
        }
      },
      Header: "操作",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        let { id, name } = value
        return (
          <Box alignX="center">
            {
              <Set>
                <EditButton id={id} name={name} onSubmitted={fetchTable} />
                <DeleteButton id={id} name={name} onSubmitted={fetchTable} />
              </Set>
            }
          </Box>
        )
      }
    })
  }
  return useMemo(() => columns)
}
