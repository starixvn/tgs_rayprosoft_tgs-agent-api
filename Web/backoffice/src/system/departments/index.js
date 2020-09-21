import React from "react"
import { css } from "@emotion/core"
import { useLocation, useRouteMatch } from "react-router-dom"
import { PageContent } from "bumbag"
import { useIsDisplayMenu } from "../../user/context"
import sequenceColumn from "../../table/column/sequence"
import AsyncTable, { useTableState } from "../../table/async"
import { Async, useAsync } from "react-async"

export const Code = "departments"

export const Content = () => {
  let { path = "" } = useRouteMatch()
  let [_, ...codes] = path.split("/")
  let isDisplay = useIsDisplayMenu(codes)
  if (!isDisplay) {
    return null
  }

  return <AsynPageContent />
}

const AsynPageContent = () => {
  let { state, setState, headSortBy, headOrderBy, request } = useTableState()
  let { pageIndex, pageSize, sortBy = [] } = state

  let fetchTable = () => {
    let promise = client.get("/api/department", {
      searchParams: {
        page: pageIndex + 1,
        rows: pageSize,
        sortby: headSortBy(),
        orderby: headOrderBy()
      }
    })
    setState({ ...state, request: promise })
    return promise
  }

  let columns = useMemo(() =>
    tableColumns({
      fetchTable
    })
  )

  useEffect(() => {
    fetchTable()
  }, [pageIndex, pageSize, sortBy])

  return (
    <PageContent title={title()}>
      <AsyncTable
        state={state}
        onStateChange={setState}
        promise={request}
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
          {/* <Set>
            <EditButton id={id} onSubmitted={fetchTable} />
            <DeleteButton id={id} name={name} onSubmitted={fetchTable} />
          </Set> */}
        </Box>
      )
    }
  }
]

//TODO: i18n
const title = () => "部门管理"
