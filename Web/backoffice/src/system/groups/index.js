import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/core"
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link,
  Redirect,
  useHistory,
  useLocation,
  useRouteMatch
} from "react-router-dom"
import client from "../../client"
import Async, { useAsync } from "react-async"
import { Formik, Field } from "formik"
import { Box, Stack, Table, Button, InputField, Set, Pagination } from "bumbag"
import PageContent from "../../page/Content"
import AsyncTable, { useTableState } from "../../table/async"
import sequenceColumn from "../../table/column/sequence"

export const Code = "groups"

export const Content = ({}) => {
  let columns = useMemo(tableColumns)
  let { state, setState, headSortBy, headOrderBy } = useTableState({ name: "" })
  let { pageIndex, pageSize, name = "", sortBy = [], request } = state

  let async = useAsync({ promise: request })
  let { isPending } = async

  useEffect(() => {
    let promise = getRoles({
      page: pageIndex + 1,
      rows: pageSize,
      name,
      sortby: headSortBy(),
      orderby: headOrderBy()
    })
    setState({ ...state, request: promise })
  }, [pageIndex, pageSize, name, sortBy])

  return (
    <PageContent
      title={title()}
      actionBar={
        <>
          <SearchForm search={setState} isPending={isPending} />
          <Button
            css={css`
              flex: 0;
            `}
          >
            TODO: 新增
          </Button>
        </>
      }
    >
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

const SearchForm = ({ search, isPending, ...props }) => {
  return (
    <Formik
      initialValues={{ name: "" }}
      onSubmit={(data) => {
        search(data)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="name"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("name", "")}>
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

const getRoles = (searchParams = {}) => {
  return client.get("/api/role", { searchParams })
}

//TODO: i18n
const title = () => "权限组管理"

//TODO: i18n
const searchButtonLabel = () => "搜索"

//TODO: i18n
const searchInputPlaceHolder = () => "权限组名称输入框"
const tableColumns = () => [
  sequenceColumn(),
  {
    Header: "权限组名称",
    id: "name",
    canSort: true,
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
    Header: "操作",
    headCell: {
      textAlign: "center"
    },
    cell: {
      textAlign: "center"
    },
    Cell: () => {
      return (
        <Set>
          <Button>TODO: 修改</Button>
          <Button>TODO: 复制</Button>
          <Button>TODO: 删除</Button>
        </Set>
      )
    }
  }
]
