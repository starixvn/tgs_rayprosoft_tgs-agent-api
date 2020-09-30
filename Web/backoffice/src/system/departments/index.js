import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import client from "../../client"
import { Box, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import { useIsDisplayMenuByRoute } from "../../user/context"
import sequenceColumn from "../../table/column/sequence"
import AsyncTable, { useTableState } from "../../table/async"
import { Async, useAsync } from "react-async"
import { AddButton, EditButton } from "./edit"
import { DeleteButton } from "./delete"
import { Code } from "./menu"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsynPageContent />
}

export const CodeContentPair = [Code, Content]

const AsynPageContent = () => {
  let { state, setState, headSortBy, headOrderBy } = useTableState()
  let { pageIndex, pageSize, sortBy = [] } = state

  let deferFn = ([data]) =>
    client.get("/api/department", {
      searchParams: {
        page: pageIndex + 1,
        rows: pageSize,
        sortby: headSortBy(),
        orderby: headOrderBy()
      }
    })

  let async = useAsync({ deferFn })
  let { run: fetchTable, promise } = async

  let columns = useMemo(() =>
    tableColumns({
      fetchTable
    })
  )

  useEffect(() => {
    fetchTable()
  }, [pageIndex, pageSize, sortBy])

  return (
    <PageContent
      title={title()}
      actionBar={
        <Box alignX="right">
          <AddButton onSubmitted={fetchTable} />
        </Box>
      }
    >
      <AsyncTable
        state={state}
        onStateChange={setState}
        promise={promise}
        head
        body
        columns={columns}
      />
    </PageContent>
  )
}

const tableColumns = ({ fetchTable }) => [
  sequenceColumn(),
  {
    Header: "部门名称",
    id: "name",
    accessor: "name",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    width: 400
  },
  {
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
  }
]

//TODO: i18n
const title = () => "部门管理"
