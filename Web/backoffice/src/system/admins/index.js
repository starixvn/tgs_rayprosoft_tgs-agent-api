import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/core"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Heading, Stack, Table, Button, InputField, Set } from "bumbag"
import PageContent from "../../page/Content"
import { AddButton, EditButton, EditPasswordButton } from "./edit"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import AsyncTable, { useTableState } from "../../table/async"
import { label as statusLabel } from "./status"
import {
  useIsDisplayMenuByRoute,
  useHasSubMenuByRoute
} from "../../user/context"
import { Code } from "./menu"

export { Code }

export const Content = () => {
  let isDisplay = useIsDisplayMenuByRoute()
  if (!isDisplay) {
    return null
  }

  return <AsyncContent />
}

export const CodeContentPair = [Code, Content]

const AsyncContent = ({}) => {
  let { state, setState, headSortBy, headOrderBy } = useTableState({
    username: ""
  })
  let { pageIndex, pageSize, username = "" } = state

  const deferFn = () =>
    client.get("/api/users", {
      searchParams: {
        page: pageIndex + 1,
        rows: pageSize,
        username,
        sortby: headSortBy(),
        orderby: headOrderBy()
      }
    })

  let async = useAsync({ deferFn })
  let { run: fetchTable, promise, isPending } = async

  let columns = useTableColumns({
    fetchTable
  })

  useEffect(() => {
    fetchTable()
  }, [pageIndex, pageSize, username])

  return (
    <PageContent
      title={title()}
      actionBar={
        <>
          <SearchForm search={setState} isPending={isPending} />
          <AddButton onSubmitted={fetchTable} />
        </>
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

const SearchForm = ({ search, isPending, ...props }) => {
  return (
    <Formik
      initialValues={{ username: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="username"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("username", "")}>
                  X
                </Button>
              }
              placeholder={searchInputPlaceHolder()}
              autocomplete="off"
            />
            <Button type="submit" isLoading={isPending}>
              {searchButtonLabel()}
            </Button>
          </Set>
        </form>
      )}
    </Formik>
  )
}

//TODO: i18n
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "管理员用户名输入框"

//TODO: i18n
const title = () => "管理员列表"

const useTableColumns = ({ fetchTable }) => {
  let columns = [
    sequenceColumn(),
    {
      Header: "用户名",
      accessor: "username",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      Header: "姓名",
      accessor: "name",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    {
      Header: "部门",
      accessor: "departmentName",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      Header: "权限组",
      accessor: "role",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center",
        whiteSpace: "nowrap"
      }
    },
    {
      Header: "最近登入IP",
      accessor: "lastLoginIp",
      disableSortBy: true,
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      }
    },
    dateTimeColumn({
      Header: "最近登入时间",
      accessor: "lastLoginTime",
      disableSortBy: true
    }),
    dateTimeColumn({
      Header: "创建时间",
      accessor: "createdOn",
      disableSortBy: true
    }),
    {
      disableSortBy: true,
      Header: "状态",
      accessor: "status",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      Cell: ({ value }) => {
        return statusLabel(value)
      }
    }
  ]

  let hasAnyMenu = useHasSubMenuByRoute()
  if (hasAnyMenu) {
    columns.push({
      disableSortBy: true,
      Header: "操作",
      headCell: {
        textAlign: "center"
      },
      cell: {
        textAlign: "center"
      },
      accessor: (row) => {
        return row
      },
      Cell: ({ value }) => {
        return (
          <Set>
            <EditButton user={value} onSubmitted={fetchTable} />
            <EditPasswordButton user={value} onSubmitted={fetchTable} />
          </Set>
        )
      }
    })
  }

  return useMemo(() => columns)
}
