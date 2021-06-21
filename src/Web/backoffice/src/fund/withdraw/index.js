import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Heading,
    Stack,
    Box,
    Button,
    Set,
    Flex,
    Level,
    FieldStack
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { Trans, useTrans } from "../../i18n"
import SearchButton from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { InputField } from "../../input/Input"
import BrandBar from "../../report/BrandBar"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import numberColumn, {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import { today } from "../../date/range"
import FundReviewStatus, {
    FundReviewStatusLabel,
    FundReviewStatusSelect,
    isFundUnderReview
} from "../FundReviewStatus"
import { DateInputRangeField } from "../../date/DateTimeRangeInput"
import { useFundWithdrawFormList } from "./client"
import { startOfNextDay } from "../../date/startOfNextDay"
import { SortOrder } from "../../table/order"
import { Update } from "./menu"
import { ViewFundWithdrawDisclosure } from "./ViewFundWithdraw"
import { VerifyFundWithdrawDisclosure } from "./VerifyFundWithdraw"
import { PaymentMethodSelect } from "../../payment/method/PaymentMethod"
import { UserTypeLabel } from "./UserType"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let todayRange = today()
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        username: "",
        channelName: "",
        status: "",
        beginCreateOn: todayRange.start,
        endCreateOn: todayRange.end,
        beginUpdateOn: undefined,
        endUpdateOn: undefined
    })

    let {
        pageIndex,
        pageSize,
        username,
        channelName,
        status,
        beginCreateOn,
        endCreateOn,
        beginUpdateOn,
        endUpdateOn
    } = state

    let {
        data: { items, ...data } = {},
        isLoading,
        mutate,
        error,
        deleteCache
    } = useFundWithdrawFormList(brandId, {
        ...toApiParams(state),
        bc: state.beginCreateOn,
        ec: startOfNextDay(state.endCreateOn),
        bu: state.beginUpdateOn,
        eu: startOfNextDay(state.endUpdateOn),
        sortby: "CreatedOn",
        orderby: SortOrder.Descending
    })

    useEffect(() => {
        setState({ pageIndex: 0 })
    }, [brandId])

    let columns = useColumns({ mutate, items, brandId })
    return (
        <PageContent
            title={<Trans zh="提款列表" en="Withdraw List" />}
            actionBar={
                <SearchBar
                    brandId={brandId}
                    username={username}
                    channelName={channelName}
                    status={status}
                    beginCreateOn={beginCreateOn}
                    endCreateOn={endCreateOn}
                    beginUpdateOn={beginUpdateOn}
                    endUpdateOn={endUpdateOn}
                    search={(data = {}) => {
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                        mutate()
                    }}
                    isLoading={isLoading}
                />
            }
        >
            <Table
                columns={columns}
                data={items}
                initialState={state}
                controlledState={state}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && <AlertDialog {...error} onClickClose={deleteCache} />}
        </PageContent>
    )
}

const SearchBar = ({
    brandId,
    username,
    channelName,
    status,
    beginCreateOn,
    endCreateOn,
    beginUpdateOn,
    endUpdateOn,
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            username,
            channelName,
            status,
            beginCreateOn,
            endCreateOn,
            beginUpdateOn,
            endUpdateOn
        }
    })

    let usernameInputPlaceholder = useTrans({
        zh: "用户名",
        en: "Username"
    })

    let channelNameInputPlaceholder = useTrans({
        zh: "通道名称",
        en: "Channel Name"
    })

    let paymentMethodPlaceholder = useTrans({
        zh: "提款类型",
        en: "Withdraw Method"
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <FieldStack orientation="vertical">
                    <Set orientation="horizontal">
                        <InputField
                            name="username"
                            showEmptyFieldValueAddonAfter
                            placeholder={usernameInputPlaceholder}
                        />
                        <InputField
                            name="channelName"
                            showEmptyFieldValueAddonAfter
                            placeholder={channelNameInputPlaceholder}
                        />
                        <PaymentMethodSelect
                            name="method"
                            placeholder={paymentMethodPlaceholder}
                        />
                        <FundReviewStatusSelect name="status" />
                    </Set>
                    <Set orientation="horizontal">
                        <DateInputRangeField
                            label={
                                <Trans zh="申请时间" en="Application Time" />
                            }
                            startProps={{
                                name: "beginCreateOn"
                            }}
                            endProps={{
                                name: "endCreateOn"
                            }}
                        />
                        <DateInputRangeField
                            label={<Trans zh="更新时间" en="Updated Time" />}
                            startProps={{
                                name: "beginUpdateOn"
                            }}
                            endProps={{
                                name: "endUpdateOn"
                            }}
                        />
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                            alignSelf="flex-end"
                        />
                    </Set>
                </FieldStack>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ brandId, items = [], mutate }) => {
    let hasUpdateMenu = useIsDisplayMenuByRoute([Update])
    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            textColumn({
                accessor: "userName",
                Header: <Trans zh="用户名" en="Username" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "userType",
                Header: <Trans zh="用户类型" en="User Type" />,
                Cell: ({ value }) => <UserTypeLabel value={value} />
            }),
            textColumn({
                accessor: "id",
                Header: <Trans zh="订单号" en="Id" />,
                disableSortBy: true,
                Cell: ({ value }) => value
            }),
            numberDecimalColumn({
                accessor: "amount",
                Header: <Trans zh="提款金额" en="Amount" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="申请时间" en="Application Time" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="更新时间" en="Update Time" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => <FundReviewStatusLabel value={value} />
            }),
            textColumn({
                accessor: "remark",
                Header: <Trans zh="备注" en="Remark" />,
                disableSortBy: true,
                Cell: ({ value }) => <Box>{value}</Box>
            })
        ]

        if (hasUpdateMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    Cell: ({ value }) => {
                        let underReview = isFundUnderReview(value?.status)
                        return (
                            <Set alignX="center">
                                {underReview && (
                                    <VerifyFundWithdrawDisclosure
                                        brandId={brandId}
                                        formId={value.id}
                                        onSubmitted={mutate}
                                    />
                                )}
                                <ViewFundWithdrawDisclosure
                                    brandId={brandId}
                                    formId={value.id}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}
