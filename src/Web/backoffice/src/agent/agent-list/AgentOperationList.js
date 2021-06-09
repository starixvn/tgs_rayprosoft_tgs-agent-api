import React, { useState, useEffect, useMemo, useRef, useContext } from "react"
import { css, Global } from "@emotion/react"
import { Spinner, Stack, Flex, Box, Card, Set } from "bumbag"
import { FieldValues } from "../../layout/Grid"
import { Trans, FormattedDateTime, FormattedNumber, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"
import { SubmitCancel } from "../../button/ActionButtons"
import { Tab as _Tab, TabPanel } from "../../tab"
import { FormProvider, useForm, useFormContext } from "react-hook-form"
import Input from "../../input/Input"
import { useAgent, useAgentOperations, useUpdateAgent } from "./client"
import { pastWeek } from "../../date/range"
import { Table, useTable, toApiParams, pageCount } from "../../table"
import dateTimeColumn from "../../table/column/dateTime"
import textColumn from "../../table/column/textColumn"
import DateTimeRangeInput from "../../date/DateTimeRangeInput"
import SearchButton from "../../button/SearchButton"
import isSameDate from "date-fns/isEqual"
import { Descending } from "../../table/order"

const Key = "AgentOperationList"

export const AgentOperationListTab = ({ ...props }) => {
    return (
        <_Tab tabId={Key} {...props}>
            <Trans zh="操作记录" en="Operating records" />
        </_Tab>
    )
}

export const AgentOperationListPanel = ({ agentId, record, ...props }) => {
    let { selectedId } = props
    let isActive = selectedId === Key

    let dateRange = pastWeek()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        from: dateRange.start,
        to: dateRange.end
    })

    let { pageSize, from, to } = state
    let { isLoading, data, error, mutate, deleteCache } = useAgentOperations(
        isActive ? agentId : undefined,
        toApiParams({ ...state, sortby: "createdOn", orderby: Descending })
    )

    let columns = useColumns({ items: data?.items })

    return (
        <TabPanel tabId={Key} {...props}>
            <Stack>
                <SearchBar
                    start={from}
                    end={to}
                    search={({ start, end, ...data } = {}) => {
                        setState({
                            pageIndex: 0,
                            from: start,
                            to: end,
                            ...data
                        })
                        mutate()
                    }}
                    isLoading={isLoading}
                />
                <Table
                    columns={columns}
                    data={data?.items}
                    pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    initialState={state}
                    controlledState={state}
                    isLoading={isLoading}
                />
            </Stack>
            {error && <AlertDialog {...error} onClickClose={deleteCache} />}
        </TabPanel>
    )
}

const SearchBar = ({ start, end, search, isLoading, ...props }) => {
    let formMethods = useForm({
        defaultValues: {
            start,
            end
        }
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <DateTimeRangeInput />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [] }) =>
    useMemo(
        () => [
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="操作时间" en="Operation Time" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "operation",
                Header: <Trans zh="操作项" en="Operation Item" />,
                disableSortBy: true,
                Cell: ({ value }) => <AgentOperationLabel value={value} />
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="操作者" en="Operator" />,

                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )

const AgentOperation = {
    Info: "Info",
    Contact: "Contact",
    Password: "Password",
    Bank: "Bank",
    Agent: "Agent"
}

const AgentOperationLabel = ({ value }) => {
    switch (value) {
        case AgentOperation.Info:
            return <Trans zh="修改代理信息" en="Edit Agent Info" />
        case AgentOperation.Contact:
            return <Trans zh="修改联系信息" en="Edit Contact Info" />
        case AgentOperation.Password:
            return <Trans zh="修改密码" en="Edit Password" />
        case AgentOperation.Bank:
            return <Trans zh="修改银行卡信息" en="Edit Bank" />
        case AgentOperation.Agent:
            return <Trans zh="修改上级" en="Edit Upper Level Agent" />
        default:
            return value
    }
}
