import React, { useState, useEffect } from "react"
import { css, Global } from "@emotion/core"
import { useAsync } from "react-async"
import dayjs from "dayjs"
import { Formik, Field } from "formik"
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
import client from "../../client"
import { faCaretUp, faCaretDown } from "@fortawesome/free-solid-svg-icons"

export const Code = "admins"

export const Content = () => {
  let table = useTable()

  return (
    <PageContent css={css``}>
      <Heading use="h3" spacing="major-3">
        {title()}
      </Heading>

      <Set orientation="vertical">
        <SearchForm table={table} />
        <AsyncDataTable table={table}>
          {({ async, data = {}, ...props }) => {
            let { items = [] } = data

            //TODO: empty table body
            return (
              <Table>
                <Table.Head>
                  <Table.Row>
                    <Sequence.HeadCell />
                    <Account.HeadCell />
                    <Name.HeadCell />
                    <Department.HeadCell />
                    <PermissionGroup.HeadCell />
                    <Ip.HeadCell />
                    <LoggedAt.HeadCell />
                    <CreatedAt.HeadCell />
                    <State.HeadCell />
                    <Action.HeadCell />
                  </Table.Row>
                </Table.Head>
                <Table.Body>
                  {items.map((row, index, list) => (
                    <Table.Row key={index}>
                      <Sequence.Cell row={row} index={index} list={list} />
                      <Account.Cell row={row} index={index} list={list} />
                      <Name.Cell row={row} index={index} list={list} />
                      <Department.Cell row={row} index={index} list={list} />
                      <PermissionGroup.Cell
                        row={row}
                        index={index}
                        list={list}
                      />
                      <Ip.Cell row={row} index={index} list={list} />
                      <LoggedAt.Cell row={row} index={index} list={list} />
                      <CreatedAt.Cell row={row} index={index} list={list} />
                      <State.Cell row={row} index={index} list={list} />
                      <Action.Cell row={row} index={index} list={list} />
                    </Table.Row>
                  ))}
                </Table.Body>
              </Table>
            )
          }}
        </AsyncDataTable>
      </Set>
    </PageContent>
  )
}

const FilterHeadCell = ({ id = "", sort = "", order = "", children }) => (
  <Table.HeadCell>
    <div
      css={css`
        display: inline-flex;
        align-items: center;
      `}
    >
      {children} <SortIcon id={id} sort={sort} order={order} />
    </div>
  </Table.HeadCell>
)

//TODO: order type? 3 state?
const SortIcon = ({ id = "", sort = "", order = "" }) => {
  if (id != sort) {
    return null
  }

  return <Icon icon={faCaretUp} type="font-awesome"></Icon>
}

//TODO: i18n
const Sequence = {
  HeadCell: () => <Table.HeadCell>序</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{index + 1}</Table.Cell>
}
const Account = {
  HeadCell: () => <Table.HeadCell>用户名</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.account}</Table.Cell>
}
const Name = {
  HeadCell: () => <Table.HeadCell>用户名</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.name}</Table.Cell>
}
const Department = {
  HeadCell: () => <Table.HeadCell>部门</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.department}</Table.Cell>
}
const PermissionGroup = {
  HeadCell: () => <Table.HeadCell>权限组</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.permissionGroup}</Table.Cell>
}
const Ip = {
  HeadCell: () => <Table.HeadCell>最近登入IP</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.ip}</Table.Cell>
}
const LoggedAt = {
  HeadCell: () => <Table.HeadCell>最近登入时间</Table.HeadCell>,
  Cell: ({ row, index, list }) => (
    <Table.Cell>{dayjs(row.loggedAt).format("YYYY/MM/DD HH/MM/SS")}</Table.Cell>
  )
}
const CreatedAt = {
  HeadCell: () => <Table.HeadCell>最近登入时间</Table.HeadCell>,
  Cell: ({ row, index, list }) => (
    <Table.Cell>
      {dayjs(row.createdAt).format("YYYY/MM/DD HH/MM/SS")}
    </Table.Cell>
  )
}
const State = {
  HeadCell: () => <Table.HeadCell>状态</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>{row.state}</Table.Cell>
}
const Action = {
  HeadCell: () => <Table.HeadCell>操作</Table.HeadCell>,
  Cell: ({ row, index, list }) => <Table.Cell>TODO: </Table.Cell>
}

const SearchForm = ({ table = {}, ...props }) => {
  let { promise, setSearch } = table
  let { isPending } = useAsync({ promise })

  return (
    <Formik
      initialValues={{ search: "" }}
      onSubmit={(data) => {
        let params = Object.entries(data)
        setSearch(params)
      }}
    >
      {(formProps) => (
        <form onReset={formProps.handleReset} onSubmit={formProps.handleSubmit}>
          <Set>
            <Field
              component={InputField.Formik}
              name="search"
              addonAfter={
                <Button onClick={() => formProps.setFieldValue("search", "")}>
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

export const AsyncDataTable = ({
  lazy,
  request,
  table,
  children,
  ...props
}) => {
  let { page, setPage, limit, sort, order, search, promise, setPromise } = table
  let { data = {}, ...async } = useAsync({
    promise
  })

  let { total = 0 } = data
  let numberOfPages = Math.ceil(total / limit)

  useEffect(() => {
    if (!request) {
      return
    }

    //TODO:
    if (lazy && page === 1) {
      return
    }

    let promise = request({
      page,
      limit,
      sort,
      order,
      search
    })

    setPromise(promise)
  }, [page, limit, sort, order, search])

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

const DefaultState = {
  page: 1,
  /* page size */
  limit: 10,
  sort: "",
  order: "",
  search: [
    /* [field, value] */
  ],
  promise: null
}

const useTable = ({ initialState = DefaultState, ...props } = {}) => {
  let [state = {}, set] = useState(initialState)

  let { page, limit, sort, order, search, promise } = state
  return {
    page,
    setPage: (page) =>
      set((prevState) => {
        return { ...prevState, page }
      }),
    limit,
    setLimit: (limit) =>
      set((prevState) => {
        return { ...prevState, limit }
      }),
    sort,
    order,
    setSortOrder: ({ sort = "", order = "" }) =>
      set((prevState) => {
        return { ...prevState, sort, order }
      }),
    search,
    setSearch: (params = []) =>
      set((prevState) => {
        return { ...prevState, search: params }
      }),
    clearSearch: () => {
      set((prevState) => {
        return { ...prevState, search: [] }
      })
    },
    promise,
    setPromise: (promise) =>
      set((prevState) => {
        return { ...prevState, promise }
      })
  }
}

//TODO: rm
const fakeResponse = {
  items: [
    {
      id: "1",
      createdAt: "2020-09-01T08:29:00.573Z",
      account: "tertiary",
      name: "Ellen",
      department: "Jewelery",
      ip: "222.10.185.50",
      loggedAt: "2020-09-02T04:21:09.558Z",
      state: "New Mexico"
    },
    {
      id: "2",
      createdAt: "2020-09-02T01:11:54.723Z",
      account: "CSS",
      name: "Dusty",
      department: "Movies",
      ip: "251.49.85.186",
      loggedAt: "2020-09-01T19:02:38.764Z",
      state: "New Hampshire"
    },
    {
      id: "3",
      createdAt: "2020-09-01T12:08:36.432Z",
      account: "Director",
      name: "Tyrique",
      department: "Industrial",
      ip: "166.69.251.175",
      loggedAt: "2020-09-01T23:21:55.015Z",
      state: "New Mexico"
    },
    {
      id: "4",
      createdAt: "2020-09-01T10:12:40.086Z",
      account: "Practical Soft Mouse",
      name: "Idella",
      department: "Industrial",
      ip: "36.34.254.230",
      loggedAt: "2020-09-01T06:21:21.562Z",
      state: "North Dakota"
    },
    {
      id: "5",
      createdAt: "2020-09-01T09:09:39.262Z",
      account: "definition",
      name: "Karl",
      department: "Health",
      ip: "172.150.14.189",
      loggedAt: "2020-09-02T01:49:41.062Z",
      state: "Nevada"
    },
    {
      id: "6",
      createdAt: "2020-09-01T19:07:45.004Z",
      account: "Handmade Frozen Computer",
      name: "Loma",
      department: "Automotive",
      ip: "73.188.145.240",
      loggedAt: "2020-09-01T07:21:30.844Z",
      state: "California"
    },
    {
      id: "7",
      createdAt: "2020-09-02T05:18:52.136Z",
      account: "payment",
      name: "Consuelo",
      department: "Home",
      ip: "182.8.144.216",
      loggedAt: "2020-09-01T13:35:06.488Z",
      state: "Missouri"
    },
    {
      id: "8",
      createdAt: "2020-09-02T04:02:04.451Z",
      account: "context-sensitive",
      name: "Boyd",
      department: "Books",
      ip: "54.127.215.150",
      loggedAt: "2020-09-02T03:39:46.391Z",
      state: "Missouri"
    },
    {
      id: "9",
      createdAt: "2020-09-01T12:03:34.932Z",
      account: "Tools",
      name: "Adonis",
      department: "Music",
      ip: "152.49.223.132",
      loggedAt: "2020-09-01T12:19:12.970Z",
      state: "Georgia"
    },
    {
      id: "10",
      createdAt: "2020-09-01T13:42:24.490Z",
      account: "cyan",
      name: "Glenda",
      department: "Toys",
      ip: "181.233.85.233",
      loggedAt: "2020-09-01T12:47:18.912Z",
      state: "New Hampshire"
    }
  ],
  count: 10,
  total: 50
}
