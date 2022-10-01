using System;

namespace m.transport.Domain
{

	public enum SubmissionStatus
	{
		UPLOAD_RUN = 0,
		UPDATE_RUN = 1,
		UPLOAD_EXCEPTION = 2,
		UPLOAD_PHOTO = 3,
		UPLOAD_DRIVER_SIGNATURE = 4,
		DELIVERY_COMPELTED = 5,
        CHECK_UPLOAD = 6
	}
}

